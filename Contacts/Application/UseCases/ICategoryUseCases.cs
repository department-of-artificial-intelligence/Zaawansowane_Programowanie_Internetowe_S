using Contacts.Application.UseCases.DTOs;

namespace Contact.Application.UseCases;

public interface ICategoryUseCases
{
    AddCategoryResult AddCategory(AddCategoryDTO addCategoryDTO);
    void RemoveCategory(int categoryId);
}