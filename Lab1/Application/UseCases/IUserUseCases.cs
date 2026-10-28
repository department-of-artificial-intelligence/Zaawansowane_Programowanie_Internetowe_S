using Application.UseCases.Dtos;
using Application.Utilities;
namespace Application.UseCases;

public interface IUserUseCases
{
  Result<AddUserResult> AddUser(AddUserDto dto);
  Result<LoginUserResult> Login(LoginUserDto dto);

}
