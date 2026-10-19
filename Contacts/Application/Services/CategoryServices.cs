using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class CategoryServices(
    ICategoryRepository categoryRepository,
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

    public Result<Unit> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        => categoryRepository.GetById(updateCategoryDto.Id)
            .Required($"Nie mogę odnaleźć kategorii o id {updateCategoryDto.Id}")
            .Bind(category =>
                categoryUniquenessChecker
                    .EnsureUnique(updateCategoryDto.Name)
                    .Bind(_ => category.Rename(updateCategoryDto.Name))
            )
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveCategory(Guid id)
    {
        if (categoryRepository.IsUsed(id))
        {
            return Result.Failure<Unit>("Nie można usunąć kategorii, ponieważ jest przypisana do adresu e-mail.");
        }

        return categoryRepository.GetById(id)
            .Required($"Nie mogę odnaleźć kategorii o id {id}")
            .Tap(categoryRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
    }
}