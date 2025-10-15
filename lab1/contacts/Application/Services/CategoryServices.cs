using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Application.UseCases;
using Application.UseCases.DTOs;
using Application.Repositories;

namespace Application.Services;
public class CategoryServices (
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork

) : ICategoryUseCases
{
    public void AddCategory(AddCategoryDTO categoryDTO)
    {
        if (categoryRepository.GetCategoryByName(categoryDTO.Name) == null)
        {
            categoryRepository.Add(categoryDTO);
            unitofWork.Save();
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
            unitofwork.Save();
        }
        else
        {
            throw new ServiceException(
                $"Kategoria_o_id: {categoryID}_nie_istnieje");
        }
    }
}
