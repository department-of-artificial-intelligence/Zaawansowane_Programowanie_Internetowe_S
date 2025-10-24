using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Domain;

namespace Application.Repositories;

public interface IUserRepository
{
    void Add(User user);

    bool UserNotExists(string userName);

    User? GetByName(string userName);

    User? GetById(int userId);
}