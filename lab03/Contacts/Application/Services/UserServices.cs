using Application.Domain;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class UserServices(
    IUserRepository userRepository,
    ITokenGenerator tokenGenerator,
    IPasswordHasher passwordHasher,
    IClock clock,
    IUnitOfWork unitOfWork,
    UserNameUniquenessChecker userNameUniquenessChecker
) : IUserUseCases
{
    public Result<AddUserResult> AddUser(AddUserDto addUserDto) =>
        addUserDto
            .ToDomain(passwordHasher)
            .Bind(user =>
                userNameUniquenessChecker
                    .EnsureUnique(user.UserName)
                    .Map(_ => user))
            .Tap(userRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddUserResult.FromDomain);

    // Rozróżniamy przyczyny niepowodzenia, ale akcja w WebAPI odpowiada
    // zawsze tym samym 401, aby nie ujawniać, czy dany login istnieje.
    public Result<LoginUserResult> Login(LoginUserDto dto)
        => userRepository.GetByName(dto.UserName ?? "")
            .Required("Błędna nazwa użytkownika")
            .Bind(user => user.VerifyPassword(dto.Password ?? "", passwordHasher)
                ? Result.Success(user)
                : Result.Failure<User>("Błędne hasło"))
            .Map(IssueTokens);

    // Zadanie 10 - refresh token jest jednorazowy: przy każdej wymianie
    // wystawiany jest nowy, a poprzedni przestaje być ważny (rotacja).
    public Result<LoginUserResult> Refresh(string refreshToken)
    {
        var tokenHash = tokenGenerator.HashRefreshToken(refreshToken ?? "");
        return userRepository
            .GetByRefreshTokenHash(tokenHash)
            .Required("Nieprawidłowy refresh token")
            .Bind(user => user.HasValidRefreshToken(tokenHash, clock.UtcNow)
                ? Result.Success(user)
                : Result.Failure<User>("Refresh token wygasł"))
            .Map(IssueTokens);
    }

    public Result<Unit> Logout(Guid userId)
        => FindUser(userId)
            .Tap(user => user.RevokeRefreshToken())
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> UpdateProfile(UpdateProfileDto dto)
        => FindUser(dto.UserId)
            .Bind(user =>
                UserProfile
                    .Create(dto.FirstName, dto.LastName, dto.DateOfBirth, dto.Address, clock.Today)
                    .Tap(user.UpdateProfile))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> ChangePassword(ChangePasswordDto dto)
        => FindUser(dto.UserId)
            .Bind(user => user.VerifyPassword(dto.CurrentPassword ?? "", passwordHasher)
                ? user.SetPassword(dto.NewPassword, passwordHasher)
                : Result.Failure<Unit>("Obecne hasło jest niepoprawne."))
            .Tap(_ => unitOfWork.Save());

    // Administrator nie może usunąć samego siebie ani odebrać sobie roli -
    // w przeciwnym razie aplikacja mogłaby zostać bez żadnego administratora.
    public Result<Unit> RemoveUser(Guid userId, Guid currentUserId)
        => NotCurrentUser(userId, currentUserId, "Nie możesz usunąć własnego konta.")
            .Bind(_ => FindUser(userId))
            .Tap(userRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> ChangeRole(ChangeRoleDto dto)
        => NotCurrentUser(dto.UserId, dto.CurrentUserId, "Nie możesz zmienić własnej roli.")
            .Bind(_ => Enum.IsDefined(dto.Role)
                ? FindUser(dto.UserId)
                : Result.Failure<User>($"Nieznana rola: {dto.Role}."))
            .Tap(user => user.ChangeRole(dto.Role))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    // Zadanie 6 - pierwszy administrator tworzony jest przy starcie aplikacji
    // na podstawie konfiguracji, kolejnych może już mianować on sam.
    public Result<Unit> EnsureAdminExists(AddUserDto dto)
        => userRepository.GetByName(dto.UserName).Match(
            _ => Result.Success(Unit.Instance),
            () => dto
                .ToDomain(passwordHasher)
                .Tap(user => user.ChangeRole(UserRole.Admin))
                .Tap(userRepository.Add)
                .Tap(_ => unitOfWork.Save())
                .Map(_ => Unit.Instance));

    private LoginUserResult IssueTokens(User user)
    {
        var refreshToken = tokenGenerator.GenerateRefreshToken();
        user.IssueRefreshToken(refreshToken.TokenHash, refreshToken.ExpiresAt);
        unitOfWork.Save();

        return new LoginUserResult(
            tokenGenerator.GenerateToken(user.Id, user.UserName, user.Role.ToString()),
            refreshToken.Token);
    }

    private static Result<Unit> NotCurrentUser(Guid userId, Guid currentUserId, string message)
        => userId != currentUserId
            ? Result.Success(Unit.Instance)
            : Result.Failure<Unit>(message);

    private Result<User> FindUser(Guid id)
        => userRepository
            .GetById(id)
            .Required($"Brak użytkownika (id: {id})");
}
