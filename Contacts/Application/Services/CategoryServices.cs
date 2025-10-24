using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;
using Contact.Application;

namespace Contacts.Application.Services;

public class CategoryServices(
    ICategoryRepository categoryRepository,
    IUnitOfWork IUnitOfWork
) : ICategoryUseCases
{
    public void AddCategory(AddCategoryDTO categoryDTO)
    {
        if (categoryRepository.GetCategoryByName(categoryDTO.Name) == null)
        {
            categoryRepository.Add(categoryDTO);
            IUnitOfWork.Save();
        }
        else
        {
            throw new ServiceException("Kategoria_o_tej_nazwie_już_istnieje");
        }
    }
    public void RemoveCategory(int categoryID)
    {
        var category = categoryRepository.GetCategory(categoryID);
        if (category != null)
        {
            categoryRepository.RemoveCategory(category);
            IUnitOfWork.Save();
        }
        else
        {
            throw new ServiceException(
                $"Kategoria_o_id:_{categoryID}_nie_istnieje");
        }
    }
}


