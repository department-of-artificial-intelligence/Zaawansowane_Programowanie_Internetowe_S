using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
using Application.DomainServices;
using Application.Domain;

namespace Application.Services;

public class UserServices(IUserRepository userRepository, ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork, 
	UserUniquenessChecker userUniquenessChecker) : IUserUseCases{

	public Result<AddUserResult> AddUser(AddUserDto addUserDto) =>
        addUserDto.ToDomain().Bind(user => userUniquenessChecker.EnsureUnique(user.UserName).Map(_ => user))
            .Bind(user => user.SetPassword(passwordHasher.HashPassword(addUserDto.Password)))
            .Tap(userRepository.Add).Tap(_ => unitOfWork.Save()).Map(AddUserResult.FromDomain);

    public Result<LoginUserResult> Login(LoginUserDto dto) => userRepository.GetByName(dto.UserName).Required("Błędna nazwa użytkownika")
        .Bind(user => passwordHasher.VerifyPassword(user.Password, dto.Password)
            ? Result.Success(IssueTokens(user)) : Result.Failure<LoginUserResult>("Błędne hasło"));

	public Result<Unit> RemoveUser(Guid id) => userRepository.GetById(id).Required("Użytkownik nie istnieje")
		.Tap(userRepository.Remove).Tap(_ => unitOfWork.Save()).Map(_ => Unit.Instance);

	public Result<Unit> UpdateProfile(UpdateProfileDto dto) => userRepository.GetById(dto.UserId).Required("Użytkownik nie istnieje")
			.Tap(user => user.UpdateProfile(dto.FirstName, dto.LastName, dto.BirthDate, dto.Address))
			.Tap(_ => unitOfWork.Save()).Map(_ => Unit.Instance);
	
	public Result<Unit> ChangeRole(Guid userId, UserRole newRole) => userRepository.GetById(userId)
        .Required("Użytkownik nie istnieje").Tap(user => user.SetRole(newRole)).Tap(_ => unitOfWork.Save())
        .Map(_ => Unit.Instance);

	private LoginUserResult IssueTokens(User user)
	{
		var accessToken = tokenGenerator.GenerateToken(user.Id, user.UserName, user.Role.ToString());
		var refreshToken = tokenGenerator.GenerateRefreshToken();
		user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
		unitOfWork.Save();
		return new LoginUserResult(accessToken, refreshToken);
	}

	public Result<LoginUserResult> RefreshToken(string refreshToken) => userRepository.GetByRefreshToken(refreshToken)
        .Required("Nieprawidłowy refresh token")
        .Bind(user => user.RefreshTokenExpiryTime > DateTime.UtcNow
            ? Result.Success(IssueTokens(user)) : Result.Failure<LoginUserResult>("Refresh token wygasł"));
}
