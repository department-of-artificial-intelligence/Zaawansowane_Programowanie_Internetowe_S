using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Queries
{
    public interface ICategoryQueries
    {
        IEnumerable<CategoryDto> GetCategories();
        Option<CategoryDto> GetCategory(Guid id);
    }
}