using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

using Contacts.Application;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;
using Contacts.Application.UseCases;

namespace Contacts.Application.Services;

public class CategoryServices(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork
) : ICategoryUseCases
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
            throw new ServiceException("Kategoria_o_tej_nazwie_juz_istnieje");
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
            throw new ServiceException(
                $"Kategoria_o_id:_{categoryID}_nie_istnieje");
        }
    }
}