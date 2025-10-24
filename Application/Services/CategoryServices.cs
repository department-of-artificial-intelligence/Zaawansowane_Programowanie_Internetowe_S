using Application.Domain; // Upewnij się, że masz tę referencję
using Application.Repositories;
using Application.UseCases.DTOs;
using System;

namespace Application.Services;

public class CategoryServices(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
{
    public Category? AddCategory(AddCategoryDTO categoryDTO)
    {
        if (categoryRepository.GetCategoryByNameAndUser(categoryDTO.Name, categoryDTO.UserId) != null)
        {
            throw new ServiceException("Kategoria o tej nazwie już istnieje");
        }

        var user = userRepository.GetById(categoryDTO.UserId);

        if (user == null)
        {
            throw new ServiceException(message: $"Użytkownik o Id: {categoryDTO.UserId} nie istnieje.");
        }

        // If we reach here, the category name is unique and the user exists.
        var category = new Category(categoryDTO.Name, user);

        categoryRepository.Add(category);
        unitOfWork.Save();

        return category;
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
            $"Kategoria o id: {categoryID} nie istnieje");
        }
    }
}