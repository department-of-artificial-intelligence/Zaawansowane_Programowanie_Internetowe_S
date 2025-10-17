using Application.Domain; // Upewnij się, że masz tę referencję
using Application.Repositories;
using Application.UseCases.DTOs;
using System;

namespace Application.Services;

public class CategoryServices(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
{
    public Category? AddCategory(AddCategoryDTO categoryDTO)
    {
        if (categoryRepository.GetCategoryByName(categoryDTO.Name) != null)
        {
            throw new ServiceException("Kategoria o tej nazwie już istnieje");
        }

        var newCategory = new Category(categoryDTO.Name);
        categoryRepository.Add(newCategory);
        unitOfWork.Save();

        return newCategory;
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