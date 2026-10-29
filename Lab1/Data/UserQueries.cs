using Application.Queries;
using Application.Queries.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UserQueries(AppDbContext context) : IUserQueries
    {
        public IEnumerable<UserDto> GetAll() =>
            context.Users
                .Select(u => new UserDto(
                    u.Id,
                    u.UserName,
                    u.FirstName,
                    u.LastName,
                    u.DateOfBirth,
                    u.Address,
                    u.Role.ToString()
                ))
                .ToList();
    }
}
