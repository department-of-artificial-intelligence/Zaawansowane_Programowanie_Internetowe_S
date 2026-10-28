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
    CategoryUniquenessChecker categoryUniquenessChecker,
    CategoryUsageChecker categoryUsageChecker
) : ICategoryUseCases
{
    public Result<AddCategoryResult> AddCategory(AddCategoryDto addCategoryDto)
        => addCategoryDto
            .ToDomain()
            .Bind(category =>
                categoryUniquenessChecker
                    .EnsureUnique(category.Name, category.UserId)
                    .Map(_ => category))
            .Tap(categoryRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddCategoryResult.FromDomain);

    public Result<Unit> RenameCategory(RenameCategoryDto renameCategoryDto)
        => categoryRepository.GetById(renameCategoryDto.Id)
            .Required($"Nie mogę odnaleźć kategorii o id = {renameCategoryDto.Id}")
            .Bind(category =>
                categoryUniquenessChecker
                    .EnsureUnique(renameCategoryDto.Name, category.UserId, category.Id)
                    .Bind(_ => category.Rename(renameCategoryDto.Name)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveCategory(Guid id)
        => categoryRepository.GetById(id)
            .Required($"Nie mogę odnaleźć kategorii o id = {id}")
            .Bind(category =>
                categoryUsageChecker
                    .EnsureNotUsed(category.Id)
                    .Map(_ => category))
            .Tap(categoryRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
}