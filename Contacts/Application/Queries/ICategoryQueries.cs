using System.Collections.Generic;

namespace Contacts.Application.Queries;

public interface ICategoryQueries
{
    IEnumerable<Contacts.Application.Queries.DTOs.CategoryDTO> GetCategories();
    Contacts.Application.Queries.DTOs.CategoryDTO? GetCategory(int categoryId);
}