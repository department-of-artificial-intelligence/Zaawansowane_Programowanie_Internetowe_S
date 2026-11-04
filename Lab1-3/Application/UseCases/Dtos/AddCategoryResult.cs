using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;

namespace Application.UseCases.Dtos;

public class AddCategoryResult(Guid id, string name)
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    internal static AddCategoryResult FromDomain(Category category) =>
    new(category.Id, category.Name);
}

