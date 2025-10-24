using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;
namespace Contacts.Application.Services;

public class CategoryServices(
ICategoryRepository categoryRepository,
IUnitOfWork unitOfWork
) : ICategoryUseCases
{
public AddCategoryResult AddCategory(AddCategoryDTO categoryDTO)    {
        if (categoryRepository.GetCategoryByName(categoryDTO.Name) == null)
        {
            categoryRepository.Add(categoryDTO);
            unitOfWork.Save();
            return new AddCategoryResult();
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie ju! istnieje");
        }
    }
    public void RemoveCategory(int categoryID)
    {
        var category = categoryRepository.GetCategory(categoryID);
        if (category == null)
        {
            throw new ServiceException(
                $"Kategoria o id: {categoryID} nie istnieje");
        }

        if (categoryRepository.IsCategoryInUse(categoryID))
        {
            throw new ServiceException(
                $"Nie można usunąć kategorii '{category.Name}', ponieważ jest przypisana do co najmniej jednego adresu email.");
        }
        categoryRepository.RemoveCategory(category);
        unitOfWork.Save();
    }

    public void UpdateCategory(UpdateCategoryDTO categoryDTO)
    {
        var existingCategoryWithName = categoryRepository.GetCategoryByName(categoryDTO.Name);
        if (existingCategoryWithName != null && existingCategoryWithName.Id != categoryDTO.Id)
        {
            throw new ServiceException("Kategoria o tej nazwie już istnieje.");
        }

        var categoryToUpdate = categoryRepository.GetCategory(categoryDTO.Id);
        if (categoryToUpdate == null)
        {
            throw new ServiceException($"Kategoria o id: {categoryDTO.Id} nie istnieje.");
        }

        categoryToUpdate.UpdateName(categoryDTO.Name);

        unitOfWork.Save();
    }
}