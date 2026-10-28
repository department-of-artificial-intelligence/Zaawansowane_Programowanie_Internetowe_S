using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos;

public class LoginUserResult(string accessToken = "")
{
    public string AccessToken { get; } = accessToken;
}
