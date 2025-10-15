using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;

namespace Contacts.Application.Services
{
    public class CategoryServices : ICategoryUseCases
    {
        public CategoryServices(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            this.categoryRepository = categoryRepository;
            this.unitOfWork = unitOfWork;
        }

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
}
