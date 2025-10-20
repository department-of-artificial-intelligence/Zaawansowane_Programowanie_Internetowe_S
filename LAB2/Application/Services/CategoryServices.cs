using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie juz istnieje");
        }
        return categoryRepository.GetCategoryByName(categoryDTO.Name);
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
