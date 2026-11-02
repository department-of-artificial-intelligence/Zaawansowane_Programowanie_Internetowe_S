using Application.Queries.Dtos;
using Application.Utilities;
namespace Application.Queries;

// Zadanie 5
public interface IUserQueries
{
    IEnumerable<UserDto> GetUsers();
    Option<UserDto> GetUser(Guid id);
}
