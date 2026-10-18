using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface ICategoryUseCases
{
    Result<AddCategoryResult> AddCategory(AddCategoryDto addCategoryDto);
    Result<Unit> RemoveCategory(Guid id);
    Result<Unit> UpdateCategory(UpdateCategoryDto updateCategoryDto);
}