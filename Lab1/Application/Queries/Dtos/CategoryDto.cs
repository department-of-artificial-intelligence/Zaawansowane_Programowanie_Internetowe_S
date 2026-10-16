using Application.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Dtos
{
    public class CategoryDto
    {
        public Guid Id { get; }
        public string Name { get; }
        private CategoryDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        public static CategoryDto FromDomain(Category category)
        => new(category.Id, category.Name);
    }
}
