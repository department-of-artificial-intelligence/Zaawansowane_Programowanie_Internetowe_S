using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;

namespace Data;

public class UserQueries(AppDbContext context) : IUserQueries
{
    public IEnumerable<UserDto> GetUsers()
        => context.Users
            .OrderBy(user => user.UserName)
            .ToList()
            .Select(UserDto.FromDomain);

    public Option<UserDto> GetUser(Guid id)
    {
        var user = context.Users.Find(id);
        return user is not null
            ? Option.Some(UserDto.FromDomain(user))
            : Option.None<UserDto>();
    }
}
