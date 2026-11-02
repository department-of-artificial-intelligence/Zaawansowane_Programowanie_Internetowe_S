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
    IContactRepository contactRepository,
    ICategoryRepository categoryRepository,
    ITokenGenerator tokenGenerator,
    IPasswordHasher passwordHasher,
    UserUniquenessChecker userUniquenessChecker,
    IUnitOfWork unitOfWork
) : IUserUseCases
{
    // Zadanie 10: token odświeżający jest ważny dłużej niż token dostępowy
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    // Zadania 2, 3, 7
    public Result<AddUserResult> AddUser(AddUserDto addUserDto) =>
        addUserDto
            .ToDomain(passwordHasher)
            .Bind(user => userUniquenessChecker
                .EnsureUnique(user.UserName)
                .Map(_ => user))
            .Tap(userRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddUserResult.FromDomain);

    // Zadania 7 i 10
    public Result<LoginUserResult> Login(LoginUserDto dto) =>
        userRepository.GetByName(dto.UserName ?? string.Empty)
            .Required("Błędna nazwa użytkownika")
            .Bind(user => user.VerifyPassword(dto.Password ?? string.Empty, passwordHasher)
                ? Result.Success(user)
                : Result.Failure<User>("Błędne hasło"))
            .Map(IssueTokens)
            .Tap(_ => unitOfWork.Save());

    // Zadanie 10: wymiana tokenu odświeżającego na nową parę tokenów.
    // Stary token odświeżający przestaje działać (rotacja).
    public Result<LoginUserResult> RefreshToken(RefreshTokenDto dto) =>
        userRepository
            .GetByRefreshTokenHash(tokenGenerator.HashRefreshToken(dto.RefreshToken ?? string.Empty))
            .Required("Nieznany token odświeżający")
            .Bind(user => user.HasValidRefreshToken(DateTime.UtcNow)
                ? Result.Success(user)
                : Result.Failure<User>("Token odświeżający wygasł"))
            .Map(IssueTokens)
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> Logout(Guid userId) =>
        GetUser(userId)
            .Tap(user => user.RevokeRefreshToken())
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    // Zadania 4 i 5
    public Result<Unit> UpdateProfile(UpdateProfileDto dto) =>
        GetUser(dto.UserId)
            .Bind(user => UserProfile
                .Create(dto.FirstName, dto.LastName, dto.BirthDate, dto.Address,
                    DateOnly.FromDateTime(DateTime.UtcNow))
                .Tap(user.UpdateProfile))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    // Zadanie 3: zmiana hasła przechodzi przez walidację w SetPassword
    public Result<Unit> ChangePassword(ChangePasswordDto dto) =>
        GetUser(dto.UserId)
            .Bind(user => user.VerifyPassword(dto.CurrentPassword ?? string.Empty, passwordHasher)
                ? Result.Success(user)
                : Result.Failure<User>("Aktualne hasło jest niepoprawne."))
            .Bind(user => user.SetPassword(dto.NewPassword ?? string.Empty, passwordHasher)
                .Map(_ => user))
            // po zmianie hasła wylogowujemy pozostałe sesje
            .Tap(user => user.RevokeRefreshToken())
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    // Zadanie 6
    public Result<Unit> ChangeRole(ChangeRoleDto dto) =>
        Validators.IsTrue(dto.UserId != dto.RequestedBy,
                "Nie możesz zmienić własnej roli.")
            .Bind(_ => GetUser(dto.UserId))
            .Tap(user => user.ChangeRole(dto.Role))
            // nowa rola trafi do tokenu przy kolejnym logowaniu lub odświeżeniu
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    // Zadanie 5: razem z użytkownikiem usuwamy jego kontakty i kategorie (zadanie 8)
    public Result<Unit> RemoveUser(RemoveUserDto dto) =>
        Validators.IsTrue(dto.UserId != dto.RequestedBy,
                "Nie możesz usunąć własnego konta.")
            .Bind(_ => GetUser(dto.UserId))
            .Tap(user =>
            {
                foreach (var contact in contactRepository.GetByUser(user.Id).ToList())
                    contactRepository.Remove(contact);
                foreach (var category in categoryRepository.GetByUser(user.Id).ToList())
                    categoryRepository.Remove(category);
                userRepository.Remove(user);
            })
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    // Zadanie 6: pierwszy administrator tworzony przy starcie aplikacji
    // (inaczej nikt nie mógłby nadać roli Admin)
    public Result<Unit> EnsureAdminExists(string userName, string password) =>
        userRepository.GetByName(userName).Match(
            _ => Result.Success(Unit.Instance),
            () => User.Create(userName, password, passwordHasher)
                .Tap(user => user.ChangeRole(UserRole.Admin))
                .Tap(userRepository.Add)
                .Tap(_ => unitOfWork.Save())
                .Map(_ => Unit.Instance));

    private Result<User> GetUser(Guid userId) =>
        userRepository.GetById(userId)
            .Required($"Brak użytkownika (id: {userId})");

    private LoginUserResult IssueTokens(User user)
    {
        var refreshToken = tokenGenerator.GenerateRefreshToken();
        user.SetRefreshToken(
            tokenGenerator.HashRefreshToken(refreshToken),
            DateTime.UtcNow.Add(RefreshTokenLifetime));

        return new LoginUserResult(
            tokenGenerator.GenerateToken(user.Id, user.UserName, user.Role.ToString()),
            refreshToken);
    }
}
