using Contacts.Application.Repositories;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Domain;

namespace Contacts.Application.Services;

public class CategoryServices(ICategoryRepository categoryRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    : ICategoryUseCases
{
    public AddCategoryResult AddCategory(AddCategoryDTO categoryDTO)
    {
        if (
            categoryRepository.GetCategoryByNameAndUser(categoryDTO.Name, categoryDTO.UserId)
            == null
        )
        {
            var user = userRepository.GetById(categoryDTO.UserId);
            if (user != null)
            {
                var category = new Category(categoryDTO.Name, user);
                categoryRepository.Add(category);
                unitOfWork.Save();
                return category;
            }
            throw new ServiceException(message: $"Użytkownik o Id: {user.Id} nie istnieje");
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie już istnieje");
        }
    }

    public EditCategoryResult EditCategory(EditCategoryDTO categoryDTO)
    {
        if (categoryRepository.GetCategoryByName(categoryDTO.Name) != null)
        {
            categoryRepository.Edit(categoryDTO);
            unitOfWork.Save();
            return categoryRepository.GetCategoryByName(categoryDTO.Name);
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie nie istnieje");
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
            throw new ServiceException($"Kategoria o id: {categoryID} nie istnieje");
        }
    }
}