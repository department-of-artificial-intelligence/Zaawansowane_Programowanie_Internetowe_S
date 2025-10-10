using Application.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories;

public interface ICategoryRepository
{
    IEnumerable<Category> GetCategories();

    Category? GetCategory(int categoryId);

    Category? GetCategoryByName(string categoryName);

    void Add(Category category);

    void RemoveCategory(Category category);
}
