using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddCategoryDTO
{
    public string Name { get; set; } = string.Empty;

    public static implicit operator Category(AddCategoryDTO dto) => new(dto.Name);
}