using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;
using Application.UseCases.DTOs;
using Contacts.Application.Domain;
namespace Contacts.Application.Services;

public class CategoryServices(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
{
    public void AddCategory(AddCategoryDTO categoryDTO)
    {
        if (categoryRepository.GetCategoryByName(categoryDTO.Name) == null)
        {
            categoryRepository.Add(categoryDTO);
            unitOfWork.Save();
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
        
        if(categoryRepository.IsCategoryUsed(categoryID) )
        {
            throw new ServiceException(
            $"Kategoria o id: {categoryID} jest używana i nie może zostać usunięta.");
        }
        categoryRepository.RemoveCategory(category);
        unitOfWork.Save();
    }

    public void UpdateCategory(UpdateCategoryDTO updateCategoryDTO)
    {
        var categoryToUpdate = categoryRepository.GetCategory(updateCategoryDTO.Id);

        if (categoryToUpdate == null)
        {
            throw new ServiceException(
            $"Kategoria o id: {updateCategoryDTO.Id} nie istnieje");
        }

        var categoryWithSameName = categoryRepository.GetCategoryByName(updateCategoryDTO.Name);
        if (categoryWithSameName != null && categoryWithSameName.Id != updateCategoryDTO.Id)
        {
            throw new ServiceException("Kategoria o tej nazwie ju! istnieje");
        }
        categoryToUpdate.UpdateName(updateCategoryDTO.Name);
        unitOfWork.Save();
    }
}