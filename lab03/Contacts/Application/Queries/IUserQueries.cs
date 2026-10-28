using Application.Queries.Dtos;
using Application.Utilities;

namespace Application.Queries;

public interface IUserQueries
{
    IEnumerable<UserDto> GetUsers();
    Option<UserDto> GetUser(Guid id);
}
