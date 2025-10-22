using Contacts.Application.Domain;
using Contacts.Application.UseCases.DTOs;
namespace Contacts.Application.UseCases;

public interface ICategoryUseCases
{
    Category AddCategory(AddCategoryDTO addCategoryDTO); 
    void RemoveCategory(int categoryId);

}
