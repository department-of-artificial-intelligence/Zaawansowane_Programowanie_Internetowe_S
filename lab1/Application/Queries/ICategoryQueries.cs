using Application.Domain;
using Application.Queries.Dtos;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Queries;

public interface ICategoryQueries
{
  IEnumerable<CategoryDto> GetCategories();
  Option<CategoryDto> GetCategory(Guid id);
  
}
