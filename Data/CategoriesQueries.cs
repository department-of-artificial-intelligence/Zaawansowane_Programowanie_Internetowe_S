using Application.Domain;
using Application.UseCases.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data;

public class CategoriesQueries(AppDbContext context) : ICategoryQueries
{
    public IEnumerable<Category> GetCategories()
    {
        return context.Categories.ToList();
    }

    public Category? GetCategory(int categoryId)
    {
        return context.Categories.Find(categoryId);
    }
}