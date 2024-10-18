using Contacts.Application.Queries;
using Contacts.Application.Queries.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Contacts.Data
{
    public class CategoriesQueries : ICategoryQueries
    {
        private readonly AppDbContext _context;

        public CategoriesQueries(AppDbContext context)
        {
            _context = context;
        }

        public List<CategoryDTO> GetAllCategories()
        {
            return _context.Categories
                .Select(category => new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name
                })
                .ToList();
        }

        public CategoryDTO? GetCategoryById(int categoryId)
        {
            var category = _context.Categories.Find(categoryId);
            if (category == null) return null;

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public CategoryDTO? GetCategoryByName(string categoryName)
        {
            var category = _context.Categories
                .FirstOrDefault(c => c.Name == categoryName);
            if (category == null) return null;

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
