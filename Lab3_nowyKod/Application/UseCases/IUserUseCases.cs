using Contacts.Application.UseCases.DTOs;
namespace Contacts.Application.UseCases;

public interface IUserUseCases
{
    AddUserResult AddUser(AddUserDTO addUserDTO);
    LoginUserResult LoginUser(LoginUserDTO loginUserDTO);
}