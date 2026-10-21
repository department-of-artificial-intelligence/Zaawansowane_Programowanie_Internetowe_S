using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface ICategoryUseCases
{
    Result<AddCategoryResult> AddCategory(AddCategoryDto addCategoryDto);
    Result<Unit> RenameCategory(Guid id, string name);
    Result<Unit> RemoveCategory(Guid id);
}
