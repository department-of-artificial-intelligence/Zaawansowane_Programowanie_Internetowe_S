using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Queries.Dtos;
using Application.Utilities;

namespace Application.Queries;

public interface ICategoryQueries
{
    IEnumerable<CategoryDto> GetCategories();
    Option<CategoryDto> GetCategory(Guid id);
}
