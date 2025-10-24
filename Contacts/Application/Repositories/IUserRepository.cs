using Contacts.Application.Domain;

namespace Contacts.Application.Repositories;

public interface IUserRepository
{
    void Add(User user);
    bool UserNotExists(string userName);
    User? GetByName(string userName);
}
