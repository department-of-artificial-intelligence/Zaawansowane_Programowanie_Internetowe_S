using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Queries.Dtos;

namespace Application.Queries
{
    public interface ICategoryQueries
    {
        IEnumerable<CategoryDto> GetCategories(Guid userId);
        Option<CategoryDto> GetCategory(Guid id, Guid userId);
    }
}
