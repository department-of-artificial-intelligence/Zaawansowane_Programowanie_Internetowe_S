using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCases.Dtos;


namespace Application.UseCases
{
    public interface ICategoryUseCases
    {
        Result<AddCategoryResult> AddCategory(AddCategoryDto addCategoryDto);
        Result<Unit> UpdateCategory(UpdateCategoryDto updateCategoryDto);
        Result<Unit> RemoveCategory(Guid id);
    }
}
