using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public class AddCategoryDto(string name, Guid userId)
{
    public string Name { get; } = name;
    public Guid UserId { get; } = userId;

    public Result<Category> ToDomain() => Category.Create(Name, UserId);
}
