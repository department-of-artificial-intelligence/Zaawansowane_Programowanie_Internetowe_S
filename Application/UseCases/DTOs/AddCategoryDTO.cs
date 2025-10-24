using Application.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.DTOs;

public class AddCategoryDTO
{
    public string Name { get; set; } = string.Empty;

    public int UserId {get; set; }

    public static implicit operator Category(AddCategoryDTO dto) => new(dto.Name);
}
