using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Domain;

namespace Application.UseCases.DTOs;

public class AddUserResult
{
    public int Id { get; init; }

    public string UserName { get; init; } = string.Empty;
    
    public static implicit operator AddUserResult(User user)
        => new(){ Id = user.Id, UserName = user.UserName};
}