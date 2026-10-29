using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;

namespace Application.UseCases.Dtos;

public record ChangeUserRoleDto(UserRole Role);
