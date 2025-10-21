using Contacts.Application.Repositories;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.Services;

public class CategoryServices(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : ICategoryUseCases
{
    public AddCategoryResult AddCategory(AddCategoryDTO categoryDTO)
    {
        if (categoryRepository.GetCategoryByName(categoryDTO.Name) == null)
        {
            categoryRepository.Add(categoryDTO);
            unitOfWork.Save();
            return categoryRepository.GetCategoryByName(categoryDTO.Name);
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie ju! istnieje");
        }
    }

    public EditCategoryResult EditCategory(EditCategoryDTO categoryDTO)
    {
        if (categoryRepository.GetCategoryByName(categoryDTO.Name) != null)
        {
            categoryRepository.Edit(categoryDTO);
            unitOfWork.Save();
            return categoryRepository.GetCategoryByName(categoryDTO.Name);
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie nie istnieje");
        }
    }

    public void RemoveCategory(int categoryID)
    {
        var category = categoryRepository.GetCategory(categoryID);
        if (category != null)
        {
            categoryRepository.RemoveCategory(category);
            unitOfWork.Save();
        }
        else
        {
            throw new ServiceException($"Kategoria o id: {categoryID} nie istnieje");
        }
    }
}
