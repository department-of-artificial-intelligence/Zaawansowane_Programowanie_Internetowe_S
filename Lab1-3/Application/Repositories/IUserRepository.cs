using Application.Domain;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories;

public interface IUserRepository
{
    void Add(User user);
    Option<User> GetByName(string userName);
    Option<User> GetById(Guid id);
    void Remove(User user);
    IEnumerable<User> GetAll();
}
