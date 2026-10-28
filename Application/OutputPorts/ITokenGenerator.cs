using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.OutputPorts;

public interface ITokenGenerator
{
    string GenerateToken(Guid userId, string userName);
}

