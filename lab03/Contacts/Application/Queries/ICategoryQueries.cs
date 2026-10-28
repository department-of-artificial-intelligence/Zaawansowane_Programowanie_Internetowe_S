using Application.Queries.Dtos;
using Application.Utilities;

namespace Application.Queries;

public interface ICategoryQueries
{
    IEnumerable<CategoryDto> GetCategories(Guid userId);

    // Zadanie 9 - kategoria jest odnajdywana tylko wśród kategorii użytkownika.
    Option<CategoryDto> GetCategory(Guid id, Guid userId);

    // Zadanie 11 - kategoria dowolnego właściciela, na potrzeby
    // autoryzacji opartej o zasoby (decyzję podejmuje IAuthorizationService).
    Option<CategoryDto> FindCategory(Guid id);
}
