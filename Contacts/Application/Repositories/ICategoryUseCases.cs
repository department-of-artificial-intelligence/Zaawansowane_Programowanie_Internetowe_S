using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application
{
    public interface ICategoryUseCases
    {
        AddCategoryResult AddCategory(AddCategoryDTO categoryDTO);
        void RemoveCategory(int categoryID);

        // --- DODAJ TĘ LINIĘ ---
        void UpdateCategory(UpdateCategoryDTO categoryDTO);
    }
}