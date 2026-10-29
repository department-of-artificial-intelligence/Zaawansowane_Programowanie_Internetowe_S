using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;

namespace Application.Queries.Dtos;

public record CategoryDto
{
    public Guid Id { get; }
    public string Name { get; }
    public Guid UserId { get; } 
    private CategoryDto(Guid id, string name, Guid userId)
    {
        Id = id;
        Name = name;
        UserId = userId;
    }

    public static CategoryDto FromDomain(Category category)
        => new(category.Id, category.Name, category.UserId);
}
