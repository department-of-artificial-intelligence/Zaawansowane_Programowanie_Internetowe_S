using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
namespace Application.Services;

public class UserServices(
IUserRepository userRepository,
ITokenGenerator tokenGenerator,
IUnitOfWork unitOfWork
) : IUserUseCases
{
  public Result<AddUserResult> AddUser(AddUserDto addUserDto) =>
  addUserDto
  .ToDomain()
  .Tap(user => user.SetPassword(addUserDto.Password))
  .Tap(userRepository.Add)
  .Tap(_ => unitOfWork.Save())
  .Map(AddUserResult.FromDomain);

  public Result<LoginUserResult> Login(LoginUserDto dto)
  => userRepository.GetByName(dto.UserName)
  .Required("Błędna nazwa użytkownika")
  .Bind(
  user => user.Password == dto.Password
  ? Result.Success(new LoginUserResult(
  tokenGenerator.GenerateToken(
  user.Id,
  user.UserName)))
  : Result.Failure<LoginUserResult>("Błędne hasło")
  );
}