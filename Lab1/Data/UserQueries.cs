using Application.Queries;
using Application.Queries.Dtos;

namespace Data;

public class UserQueries(AppDbContext context) : IUserQueries{
    public IEnumerable<UserDto> GetUsers() => context.Users.ToList().Select(UserDto.FromDomain);
}
