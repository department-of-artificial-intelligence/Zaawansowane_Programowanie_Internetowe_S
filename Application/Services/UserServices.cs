using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Domain;
using Application.DomainServices;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.DTOs;

namespace Application.Services;

public class UserServices(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ITokenGenerator tokenGenerator
) : IUserUseCases
{
    public AddUserResult AddUser(AddUserDTO addUserDTO)
    {
        var user = new User(addUserDTO.UserName);
        user.SetPassword(addUserDTO.Password);

        userRepository.Add(user);

        return user;
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