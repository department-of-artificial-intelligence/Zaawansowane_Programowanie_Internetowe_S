using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;

namespace Contacts.Application.Services;

public class CategoryServices(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork
) : ICategoryUseCases
{
    public void AddCategory(AddCategoryDTO categoryDTO)
    {
        if(categoryRepository.GetCategoryByName(categoryDTO.Name) == null)
        {
            categoryRepository.Add(categoryDTO);
            unitOfWork.Save();
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie nie instnieje");
        }
    }

    public void RemoveCategory(int categoryID)
    {
        var category = categoryRepository.GetCategory(categoryID);
        if(category != null)
        {
            categoryRepository.RemoveCategory(category);
            unitOfWork.Save();
        }
        else
        {
            throw new ServiceException($"Kategoria o id:{categoryID} nie istnieje");
        }
    }

    AddCategoryResult ICategoryUseCases.AddCategory(AddCategoryDTO addCategoryDTO)
    {
        throw new NotImplementedException();
    }
}