using Application.OutputPorts;
using Application.Domain;
using Application.DomainServices;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class UserServices(
    IUserRepository userRepository,
    ITokenGenerator tokenGenerator,
    IPasswordHasher passwordHasher,
    UserUniquenessChecker uniquenessChecker,
    IUnitOfWork unitOfWork
) : IUserUseCases
{
    public Result<AddUserResult> AddUser(AddUserDto addUserDto) =>
        addUserDto
            .ToDomain()
            .Bind(user => uniquenessChecker
                .EnsureUnique(addUserDto.UserName)
                .Map(_ => user))
            .Bind(user => user
                .SetPassword(passwordHasher.Hash(addUserDto.Password))
                .Map(_ => user))
            .Tap(userRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddUserResult.FromDomain);

    public Result<LoginUserResult> Login(LoginUserDto dto) =>
        userRepository.GetByName(dto.UserName)
            .Required("Błędna nazwa użytkownika")
            .Bind(user => passwordHasher.Verify(user.Password, dto.Password)
                ? Result.Success(new LoginUserResult(
                    tokenGenerator.GenerateToken(
                        user.Id,
                        user.UserName,
                        user.Role.ToString()),
                    CreateRefreshToken(user)))
                : Result.Failure<LoginUserResult>("Błędne hasło"));

    public Result<LoginUserResult> Refresh(RefreshTokenDto dto)
    {
        var user = userRepository.GetAll()
            .SingleOrDefault(value => value.RefreshToken == dto.RefreshToken);
        if (user is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            return Result.Failure<LoginUserResult>("Refresh token jest nieprawidłowy lub wygasł.");

        return Result.Success(new LoginUserResult(
            tokenGenerator.GenerateToken(user.Id, user.UserName, user.Role.ToString()),
            CreateRefreshToken(user)));
    }

    private string CreateRefreshToken(User user)
    {
        var token = tokenGenerator.GenerateRefreshToken();
        user.SetRefreshToken(token, DateTime.UtcNow.AddDays(30));
        unitOfWork.Save();
        return token;
    }

    public IEnumerable<UserDto> GetAll() => userRepository.GetAll().Select(UserDto.FromDomain);

    public Result<Unit> UpdateProfile(Guid id, UpdateProfileDto dto) =>
        userRepository.GetById(id)
            .Required("Użytkownik nie istnieje")
            .Bind(user => user.UpdateProfile(dto.FirstName, dto.LastName, dto.BirthDate, dto.Address))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> ChangeRole(Guid id, UserRole role) =>
        userRepository.GetById(id)
            .Required("Użytkownik nie istnieje")
            .Tap(user => user.SetRole(role))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> Remove(Guid id) =>
        userRepository.GetById(id)
            .Required("Użytkownik nie istnieje")
            .Tap(userRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
}
