using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.Application.Domain;

namespace Contacts.Application.Respositories
{
    public interface ICategoryServices
    {
        IEnumerable<Category> GetCategories();
        Category? GetCategory(int categoryId);
        Category? GetCategoryByName(string categoryName);
        void Add(Category category);
        void RemoveCategory(Category category);
    }
}