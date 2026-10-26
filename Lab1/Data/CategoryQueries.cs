using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class CategoryQueries(AppDbContext context) : ICategoryQueries
    {
        public IEnumerable<CategoryDto> GetCategories()
        => context.Categories.ToList().Select(CategoryDto.FromDomain);
        public Option<CategoryDto> GetCategory(Guid id)
        {
            var category = context.Categories.Find(id);
            return category is not null
            ? Option.Some(CategoryDto.FromDomain(category))
            : Option.None<CategoryDto>();
        }
    }

}