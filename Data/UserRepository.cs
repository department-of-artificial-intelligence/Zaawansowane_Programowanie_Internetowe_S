using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Domain;
using Application.Repositories;

namespace Data;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public bool UserNotExists(string userName)
        => context.Users.SingleOrDefault(u => u.UserName == userName) is null;

    public void Add(User user)
    {
        context.Users.Add(user);
        context.SaveChanges();
    }

    public User? GetByName(string userName)
        => context.Users.SingleOrDefault(user => user.UserName == userName);

    public User? GetById(int userId)
    {
        return context.Users.FirstOrDefault(x => x.Id == userId);
    }
}