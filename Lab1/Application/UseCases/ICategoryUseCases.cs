using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public interface ICategoryUseCases
    {
        Result<AddCategoryResult> AddCategory(AddCategoryDto addCategoryDto);
        Result<Uint> RemoveCategory(Guid id);
    }
}