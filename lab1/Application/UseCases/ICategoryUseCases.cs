using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface ICategoryUseCases
{
  Result<AddCategoryResult> AddCategory()
}
