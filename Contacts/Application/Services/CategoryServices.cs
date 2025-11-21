using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;
using Contacts.Application.Domain;
namespace Contacts.Application.Services;

public class CategoryServices(
ICategoryRepository categoryRepository,
IUnitOfWork unitOfWork
) : ICategoryUseCases
{
public AddCategoryResult AddCategory(AddCategoryDTO categoryDTO)
{
    // 1. Walidacja
    if (categoryRepository.GetCategoryByName(categoryDTO.Name) != null)
    {
        throw new ServiceException("Kategoria o tej nazwie już istnieje");
    }

    // 2. Tworzenie encji
    var category = new Category(categoryDTO.Name);
    
    // 3. Dodanie do repozytorium
    categoryRepository.Add(category); 
    
    // 4. Zapis
    unitOfWork.Save();

    // 5. POPRAWKA: Zwróć całą encję 'category'.
    // Twoja klasa AddCategoryResult automatycznie
    // przekonwertuje ją na DTO (dzięki 'implicit operator').
    return category;
}
    public void RemoveCategory(int categoryID)
    {
        // 1. Sprawdź, czy kategoria w ogóle istnieje
        var category = categoryRepository.GetCategory(categoryID);
        if (category == null)
        {
            throw new ServiceException(
                $"Kategoria o id: {categoryID} nie istnieje");
        }

        // 2. NOWY KROK: Sprawdź, czy kategoria jest w użyciu
        if (categoryRepository.IsCategoryInUse(categoryID))
        {
            // Jeśli tak, zablokuj usunięcie i poinformuj użytkownika
            throw new ServiceException(
                $"Nie można usunąć kategorii '{category.Name}', ponieważ jest przypisana do co najmniej jednego adresu email.");
        }
        
        // 3. Jeśli warunki są spełnione (nie jest w użyciu) - usuń
        categoryRepository.RemoveCategory(category);
        unitOfWork.Save();
    }

    AddCategoryResult ICategoryUseCases.AddCategory(AddCategoryDTO addCategoryDTO)
    {
        throw new NotImplementedException();
    }
        public void UpdateCategory(UpdateCategoryDTO categoryDTO)
    {
        // 1. Sprawdź, czy kategoria o docelowej nazwie już nie istnieje
        // (i czy nie jest to ta sama kategoria, którą edytujemy)
        var existingCategoryWithName = categoryRepository.GetCategoryByName(categoryDTO.Name);
        if (existingCategoryWithName != null && existingCategoryWithName.Id != categoryDTO.Id)
        {
            throw new ServiceException("Kategoria o tej nazwie już istnieje.");
        }

        // 2. Znajdź kategorię, którą chcemy zaktualizować
        var categoryToUpdate = categoryRepository.GetCategory(categoryDTO.Id);
        if (categoryToUpdate == null)
        {
            throw new ServiceException($"Kategoria o id: {categoryDTO.Id} nie istnieje.");
        }

        // 3. Wywołaj metodę domenową (walidacja nazwy odbędzie się w encji)
        categoryToUpdate.UpdateName(categoryDTO.Name);

        // 4. Zapisz zmiany
        // Nie musimy wywoływać `repository.Update()`,
        // ponieważ EF Core śledzi zmiany na pobranej encji.
        unitOfWork.Save();
    }
}