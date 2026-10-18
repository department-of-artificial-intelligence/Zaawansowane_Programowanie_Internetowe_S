
using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;


public class CategoryServices(
    ICategoryRepository categoryRepository,
    IContactRepository contactRepository,
    IUnitOfWork unitOfWork,
    CategoryUniquenessChecker categoryUniquenessChecker
) : ICategoryUseCases

{
    public Result<AddCategoryResult> AddCategory(AddCategoryDto addCategoryDto)
        => addCategoryDto
            .ToDomain()
            .Bind(category =>
                categoryUniquenessChecker
                    .EnsureUnique(category.Name)
                    .Map(_ => category)
            )
            .Tap(categoryRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddCategoryResult.FromDomain);

    
    public Result<Unit> RemoveCategory(Guid id)
    {
        var category = categoryRepository.GetById(id);

        return category
            .Required($"Nie znaleziono kategorii o ID: {id}")
            .Bind(existingCategory =>
            {
                if (contactRepository.IsCategoryUsed(id))
                {
                    return Result.Failure<Unit>(
                        "Nie można usunąć kategorii, ponieważ jest przypisana do adresów e-mail.");
                }

                categoryRepository.Remove(existingCategory);
                unitOfWork.Save();

                return Result.Success(Unit.Instance);
            });
    }


    
    public Result<Unit> UpdateCategory(UpdateCategoryDto dto)
    {
        return categoryRepository.GetById(dto.Id)
            .Required($"Nie mogę odnaleźć kategorii o id = {dto.Id}")
            .Bind(category =>
            {
                if (category.Name == dto.Name.Trim())
                    return Result.Success(Unit.Instance);

                return categoryUniquenessChecker
                    .EnsureUnique(dto.Name.Trim())
                    .Bind(_ => category.ChangeName(dto.Name));
            })
            .Tap(_ => unitOfWork.Save());
    }

}
