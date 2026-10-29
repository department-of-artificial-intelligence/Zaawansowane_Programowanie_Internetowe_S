using Application.Domain;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos
{
    public class AddUserDto(string userName, string password)
    {
        public string UserName { get; } = userName;
        public string Password { get; } = password;

        // Przekazanie obu parametrów do smart konstruktora
        public Result<User> ToDomain() => User.Create(UserName, Password);
    }
}
