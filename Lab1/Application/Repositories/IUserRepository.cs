using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IUserRepository
{
    void Add(User user);
    Option<User> GetByName(string userName);
    Option<User> GetById(Guid id);
    IEnumerable<User> GetAll();
    void Remove(User user);
}
