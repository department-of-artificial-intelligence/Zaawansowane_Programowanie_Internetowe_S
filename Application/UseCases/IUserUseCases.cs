using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases
{
    public interface IUserUseCases
    {
        Result<AddUserResult> AddUser(AddUserDto dto);
        Result<LoginUserResult> Login(LoginUserDto dto);
    }
}
