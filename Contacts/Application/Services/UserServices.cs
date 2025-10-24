using Contacts.Application.Domain;
using Contacts.Application.DomainServices;
using Contacts.Application.Repositories;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.Services;

public class UserServices(
    IUserRepository userRepository,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork
) : IUserUseCases
{
    public AddUserResult AddUser(AddUserDTO userDTO)
    {
        if (userRepository.UserNotExists(userDTO.UserName))
        {
            var user = new User(userDTO.UserName);
            user.SetPassword(userDTO.Password);
            userRepository.Add(user);
            unitOfWork.Save();
            return user;
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie już istnieje");
        }
    }

    public LoginUserResult LoginUser(LoginUserDTO loginUserDTO)
    {
        var user = userRepository.GetByName(loginUserDTO.UserName);
        if (user is not null)
        {
            if (user.Password == loginUserDTO.Password)
            {
                var token = tokenGenerator.GenerateToken(user.Id, user.UserName);
                return new LoginUserResult { Status = UserLoginStatus.UserLogged, Token = token };
            }
            else
            {
                return new LoginUserResult { Status = UserLoginStatus.WrongUserOrPassword };
            }
        }
        else
        {
            return new LoginUserResult { Status = UserLoginStatus.WrongUserOrPassword };
        }
    }
}
