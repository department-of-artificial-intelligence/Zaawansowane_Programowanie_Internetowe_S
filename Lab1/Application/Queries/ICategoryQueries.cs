using Contacts.Application.Queries.DTOs;
using System.Collections.Generic;

namespace Contacts.Application.Queries
{
    public interface ICategoryQueries
    {
        List<CategoryDTO> GetAllCategories();       
        CategoryDTO? GetCategoryById(int categoryId); 
        CategoryDTO? GetCategoryByName(string categoryName); 
    }
}
