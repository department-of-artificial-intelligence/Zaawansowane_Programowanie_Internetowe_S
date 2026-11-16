using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos
{
    public class DeleteCategoryDto(Guid id)
    {
        public Guid Id { get; set; } = id;
    }
}