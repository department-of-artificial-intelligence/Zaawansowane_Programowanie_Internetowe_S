using Application.Domain;
using Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public Category? Add(Category category)
    {
        context.Categories.Add(category);

        return category;
    }

    public IEnumerable<Category> GetCategories()=> context.Categories;

    public Category? GetCategory(int categoryId) => context.Categories.Find(categoryId);

    public Category? GetCategoryByName(string categoryName) => context.Categories.SingleOrDefault(category => category.Name == categoryName);

    public void RemoveCategory(Category category) => context.Remove(category);
}
