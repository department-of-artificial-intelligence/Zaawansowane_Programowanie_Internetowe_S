using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.UseCases;

public interface ICategoryUseCases
{
    void AddCategory(AddCategoryDTO addCategoryDTO);
    void RemoveCategory(int categoryID);
}
