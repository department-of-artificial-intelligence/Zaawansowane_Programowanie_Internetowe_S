using Contacts.Application.Queries;
using Contacts.Application.Queries.DTOs;

namespace Contacts.Data
{
    public class CategoriesQueries : ICategoryQueries
    {
        private readonly AppDbContext context;

        public CategoriesQueries(AppDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<CategoryDTO> GetCategories()
        {
            return context.Categories
                .ToList()
                .Select(category => (CategoryDTO)category);
        }

        public CategoryDTO? GetCategory(int categoryId)
        {
            return context.Categories.Find(categoryId);
        }
    }
}
