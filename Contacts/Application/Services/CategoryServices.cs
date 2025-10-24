using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;
using Contacts.Application; // for IUnitOfWork
using Contacts.Application.Domain; // for Category

namespace Contacts.Application.Services;

public class CategoryServices(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork) : ICategoryUseCases
{
    public AddCategoryResult AddCategory(AddCategoryDTO categoryDTO)
    {
        if (categoryRepository.GetCategoryByName(categoryDTO.Name) == null)
        {
            var category = (Category)categoryDTO;
            categoryRepository.Add(category);
            unitOfWork.Save();
            return (AddCategoryResult)category;
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie juz istnieje");

        }
    }
    public void RemoveCategory(int categoryID)
    {
        var category = categoryRepository.GetCategory(categoryID);
        if (category != null)
        {
            // Prevent deletion if any emails reference this category
            if (categoryRepository.IsCategoryInUse(categoryID))
            {
                throw new ServiceException("Nie można usunąć kategorii, ponieważ przypisane są do niej adresy email");
            }
            categoryRepository.RemoveCategory(category);
            unitOfWork.Save();
        }
        else
        {
            throw new ServiceException("Kategoria o podanym ID nie istnieje");
        }
    }
}