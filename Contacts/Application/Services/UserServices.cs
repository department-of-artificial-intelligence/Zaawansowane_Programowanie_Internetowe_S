using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
using Application.DomainServices; 
using Application.Queries.Dtos;
using Application.Domain;
namespace Application.Services;
public class UserServices(
    IUserRepository userRepository,
    ITokenGenerator tokenGenerator,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    UserUniquenessChecker userUniquenessChecker
) : IUserUseCases
{
    public Result<AddUserResult> AddUser(AddUserDto addUserDto) =>
        addUserDto
            .ToDomain(passwordHasher)
            .Bind(user=>
                    userUniquenessChecker
                        .EnsureUnique(user.UserName)
                        .Map(_=>user))
            .Tap(userRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddUserResult.FromDomain);
    public Result<LoginUserResult> Login(LoginUserDto dto)
        => userRepository.GetByName(dto.UserName)
            .Required("Błędna nazwa użytkownika")
            .Bind(
                user => passwordHasher.VerifyHashedPassword(user.Password, dto.Password)
                    ? Result.Success(new LoginUserResult(
                    tokenGenerator.GenerateToken(
                        user.Id,
                        user.UserName, user.Role.ToString())))
                : Result.Failure<LoginUserResult>("Błędne hasło")
            );

    public Result<IEnumerable<UserDto>> GetUsers()
        => Result.Success(userRepository.GetAll().Select(UserDto.FromDomain));

    public Result<Unit> UpdateUser(Guid userId, UpdateUserDto dto)
        => userRepository.GetById(userId)
            .Required($"Nie mogę odnaleźć użytkownika o id {userId}")
            .Bind(user =>
                FirstName.Create(dto.FirstName)
                    .Bind(firstName => LastName.Create(dto.LastName)
                        .Tap(lastName => user.UpdateUser(firstName, lastName, dto.DateOfBirth, dto.Address))
                    )
            )
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> RemoveUser(Guid id)
        => userRepository.GetById(id)
            .Required($"Nie mogę odnaleźć użytkownika o id {id}")
            .Tap(userRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> ChangeRole(Guid userId, ChangeUserRoleDto dto)
        => userRepository.GetById(userId)
            .Required($"Nie mogę odnaleźć użytkownika o id {userId}")
            .Tap(user => user.SetRole(dto.Role))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<TokenResponseDto> RefreshToken(RefreshTokenDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RefreshToken))
        {
            return Result.Failure<TokenResponseDto>("Brak tokena odświeżającego.");
        }

        var user = userRepository.GetAll()
            .FirstOrDefault(u => u.RefreshToken == dto.RefreshToken);

        if (user is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Result.Failure<TokenResponseDto>("Token odświeżający jest nieprawidłowy lub wygasł.");
        }

        var newAccessToken = tokenGenerator.GenerateToken(user.Id, user.UserName, user.Role.ToString());
        var newRefreshToken = tokenGenerator.GenerateRefreshToken();

        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        unitOfWork.Save();

        return Result.Success(new TokenResponseDto(newAccessToken, newRefreshToken));
    }
}
