using Application.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.DTOs;

public class AddCategoryResult
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public static implicit operator AddCategoryResult(Category category) => new AddCategoryResult { Id = category.Id, Name = category.Name };
}
