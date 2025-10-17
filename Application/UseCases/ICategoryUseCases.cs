using Application.UseCases.DTOs;
using Contacts.Application.UseCases.DTOs;
namespace Contacts.Application.UseCases;

public interface ICategoryUseCases
{
    AddCategoryResult AddCategory(AddCategoryDTO addCategoryDTO);
    void UpdateCategory(UpdateCategoryDTO updateCategoryDTO);
    void RemoveCategory(int categoryId);

}