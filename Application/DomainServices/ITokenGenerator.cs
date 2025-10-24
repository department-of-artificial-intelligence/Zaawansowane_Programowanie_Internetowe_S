using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DomainServices;

public interface ITokenGenerator
{
    string GenerateToken(int userId, string userName);
}