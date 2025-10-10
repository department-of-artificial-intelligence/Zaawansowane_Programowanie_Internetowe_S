using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Domain;

namespace Application.UseCases.DTOs
{
    public class AddCategoryDTOs
    {
        public string Name { get; set; } = string.Empty;
        public static implicit operator Category(AddCategoryDTOs dto) => new(dto.Name);
    }
}