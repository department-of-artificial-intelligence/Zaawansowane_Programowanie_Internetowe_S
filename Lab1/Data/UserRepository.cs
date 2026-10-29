using Application.Domain;
using Application.Repositories;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public Option<User> GetByName(string userName)
        {
            var user = context.Users.SingleOrDefault(user => user.UserName == userName);
            return user is not null
                ? Option.Some(user)
                : Option.None<User>();
        }
        public void Add(User user)
        {
            context.Users.Add(user);
        }

        public Option<User> GetById(Guid id)
        {
            var user = context.Users.Find(id);
            return user is not null ? Option.Some(user) : Option.None<User>();
        }

        public void Remove(User user)
        {
            context.Users.Remove(user);
        }

        public Option<User> GetByRefreshToken(string refreshToken)
        {
            var user = context.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);
            return user is not null ? Option.Some(user) : Option.None<User>();
        }
    }
}
