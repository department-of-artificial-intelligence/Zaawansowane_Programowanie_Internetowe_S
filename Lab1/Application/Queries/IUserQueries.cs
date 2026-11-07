using Application.Queries.Dtos;

namespace Application.Queries;

public interface IUserQueries
{
    IEnumerable<UserDto> GetUsers();
}