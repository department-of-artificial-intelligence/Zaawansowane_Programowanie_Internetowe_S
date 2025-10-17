using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.DTOs;

public class UpdateCategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

}
