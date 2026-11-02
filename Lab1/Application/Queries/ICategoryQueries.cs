using Application.Queries.Dtos;
using Application.Utilities;
namespace Application.Queries;

public interface ICategoryQueries
{
    IEnumerable<CategoryDto> GetCategories(Guid userId);

    // Zadanie 9: kategoria tylko dla określonego użytkownika
    Option<CategoryDto> GetCategory(Guid id, Guid userId);

    // Zadanie 11: właściciel kategorii (do autoryzacji opartej o zasoby)
    Option<Guid> GetCategoryOwner(Guid id);
}
