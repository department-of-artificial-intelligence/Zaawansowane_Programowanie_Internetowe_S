using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.Application.UseCases.DTOs;

namespace Contacts.Data;

public class CategoriesQueries(AppDbContext context) : ICategoryQueries
{
    public IEnumerable<CategoryDTO> GetCategories()
    {
        return context.Categories.ToList().Select(
            category => (CategoryDTO)category);
    }

    public CategoryDTO? GetCategory(int categoryId)
    {
        return context.Categories.Find(categoryId);
    } 
}