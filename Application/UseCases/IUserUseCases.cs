using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.UseCases.DTOs;

namespace Application.UseCases;

public interface IUserUseCases
{
    AddUserResult AddUser(AddUserDTO addUserDTO);  

    LoginUserResult LoginUser(LoginUserDTO loginUserDTO); 
}