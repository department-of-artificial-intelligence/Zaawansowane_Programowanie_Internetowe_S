using Application.Domain;
namespace Application.Repositories;
public interface IUserRepository
{
    void Add(User user);
    Option<User> GetByName(string userName);
}