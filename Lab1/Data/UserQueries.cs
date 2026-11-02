using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;
using Microsoft.EntityFrameworkCore;
namespace Data;

public class UserQueries(AppDbContext context) : IUserQueries
{
    public IEnumerable<UserDto> GetUsers() =>
        context.Users
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .ToList()
            .Select(UserDto.FromDomain);

    public Option<UserDto> GetUser(Guid id)
    {
        var user = context.Users.AsNoTracking().SingleOrDefault(user => user.Id == id);
        return user is not null
            ? Option.Some(UserDto.FromDomain(user))
            : Option.None<UserDto>();
    }
}
