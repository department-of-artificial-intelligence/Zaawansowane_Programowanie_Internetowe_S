using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;
using Contacts.Application.Domain;
namespace Contacts.Application.Services;

public class CategoryServices(
  ICategoryRepository categoryRepository,
  IUnitOfWork unitOfWork)
   : ICategoryUseCases
{
    public Category AddCategory(AddCategoryDTO categoryDTO)
    {
        // Check if category with the same name already exists
        if (categoryRepository.GetCategoryByName(categoryDTO.Name) == null)
        {
            // Create the Category using only the name, ID will default to 0 initially
            var category = new Category(categoryDTO.Name); // Uses the constructor (name, 0)

            categoryRepository.Add(category); // Add the category to the repository
            unitOfWork.Save(); // Save the changes

            return category; // Return the created category
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie już istnieje");
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
               $"Kategoria o id: {categoryID} nie istnieje");
        }
    }


}