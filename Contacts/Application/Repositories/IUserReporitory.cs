using Application.Domain;
using Application.Utilities;
namespace Application.Repositories;
public interface IUserRepository
{
    void Add(User user);
    Option<User> GetByName(string userName);
    IEnumerable<User> GetAll();
    Option<User> GetById(Guid id);
    void Remove(User user);
}