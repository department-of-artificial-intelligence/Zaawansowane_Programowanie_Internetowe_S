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

    public Result<Unit> RenameCategory(RenameCategoryDto renameCategoryDto)
        => categoryRepository.GetById(renameCategoryDto.CategoryId)
            .Required($"Nie mogę odnaleźć kategorii o id = {renameCategoryDto.CategoryId}")
            .Rename(renameCategoryDto.Name)
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveCategory(Guid id)
        => categoryRepository.GetById(id)
            .Required($"Nie mogę odnaleźć kategorii o id = {id}")
            .Ensure(
                _ => !contactRepository.HasEmail(id),
                "Nie można usunąć kategorii, ponieważ przypisano do niej adresy e-mail."
            )
            .Tap(categoryRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
}