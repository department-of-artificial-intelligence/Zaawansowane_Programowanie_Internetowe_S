using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Domain;

public class User
{
    public int Id { get; private set; }

    public string UserName { get; private set; } = string.Empty;

    public string Password { get; private set; } = string.Empty;

    private User()
    {
    }

    public User(string userName)
    {
        UserName = userName;
    }
    
    public void SetPassword(string password)
    {
        Password = password;
    }
}