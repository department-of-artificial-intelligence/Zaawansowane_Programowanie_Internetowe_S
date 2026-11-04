using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos;

public record LoginUserResult(string AccessToken, string RefreshToken);
