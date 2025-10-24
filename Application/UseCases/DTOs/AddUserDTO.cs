using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.DTOs;

public class AddUserDTO
{
    public string UserName { get; set; } = string.Empty;
    
    public string Password {get; set; } = string.Empty;
}