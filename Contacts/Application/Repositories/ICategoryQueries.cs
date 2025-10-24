using Contacts.Application.UseCases.DTOs;
using System.Collections.Generic;

namespace Contacts.Application.Queries
{
    public interface ICategoryQueries
    {
        IEnumerable<CategoryDTO> GetCategories();
        
        CategoryDTO? GetCategory(int categoryId);
    }
}