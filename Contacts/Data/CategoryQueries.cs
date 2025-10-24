using System.Collections.Generic;
using System.Linq;
using Contacts.Application.Queries;
using Microsoft.EntityFrameworkCore;
using CategoryDTOAlias = Contacts.Application.Queries.DTOs.CategoryDTO;

namespace Contacts.Data;

public class CategoryQueries(AppDbContext context) : ICategoryQueries
{
    public IEnumerable<Contacts.Application.Queries.DTOs.CategoryDTO> GetCategories()
    {
        return context.Categories
            .AsNoTracking()
            .Select(c => new CategoryDTOAlias(c.Id, c.Name))
            .ToList();
    }

    public Contacts.Application.Queries.DTOs.CategoryDTO? GetCategory(int categoryId)
    {
        var category = context.Categories
            .AsNoTracking()
            .FirstOrDefault(c => c.Id == categoryId);
        return category is null ? null : new CategoryDTOAlias(category.Id, category.Name);
    }
}