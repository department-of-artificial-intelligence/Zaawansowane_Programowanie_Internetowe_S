using Application.UseCases.DTOs;

namespace Application.UseCases;

public interface ICategoryUseCases
{
    AddCategoryResult AddCategory(AddCategoryDTO addCategoryDTO);

    void RemoveCategory(int categoryId);
}