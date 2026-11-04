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
    UserUniquenessChecker userUniquenessChecker,
    IUnitOfWork unitOfWork

) : IUserUseCases
{

    public Result<AddUserResult> AddUser(AddUserDto addUserDto) =>
        addUserDto
            .ToDomain(passwordHasher.Hash)
            .Bind(user =>
                userUniquenessChecker
                    .EnsureUnique(user.UserName)
                    .Map(_ => user))
            .Tap(userRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddUserResult.FromDomain);

    public Result<LoginUserResult> Login(LoginUserDto dto)

        => userRepository.GetByName(dto.UserName)
            .Required("Błędna nazwa użytkownika")
            .Bind(
                user => passwordHasher.Verify(user.Password, dto.Password)
                    ? Result.Success(user)
                    : Result.Failure<User>("Błędne hasło")
            )
            .Bind(IssueTokens);

    public Result<LoginUserResult> Refresh(RefreshTokenDto dto)

        => userRepository.GetByRefreshToken(dto.RefreshToken)
            .Required("Błędny token odświeżający")
            .Bind(
                user => user.RefreshTokenExpiresAt > DateTime.UtcNow
                    ? Result.Success(user)
                    : Result.Failure<User>("Token odświeżający wygasł")
            )
            .Bind(IssueTokens);

    public Result<Unit> RemoveUser(Guid id)
        => userRepository.GetById(id)
            .Required($"Brak użytkownika (id: {id})")
            .Tap(userRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> UpdateProfile(UpdateUserProfileDto dto)
        => userRepository.GetById(dto.Id)
            .Required($"Brak użytkownika (id: {dto.Id})")
            .Bind(dto.ApplyTo)
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> ChangeRole(ChangeUserRoleDto dto)
        => userRepository.GetById(dto.Id)
            .Required($"Brak użytkownika (id: {dto.Id})")
            .Tap(user => user.ChangeRole(dto.Role))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    private Result<LoginUserResult> IssueTokens(User user)
    {
        user.SetRefreshToken(
            tokenGenerator.GenerateRefreshToken(),
            DateTime.UtcNow.AddDays(7));
        unitOfWork.Save();

        return Result.Success(new LoginUserResult(
            tokenGenerator.GenerateToken(
                user.Id,
                user.UserName,
                user.Role.ToString()),
            user.RefreshToken));
    }

}
