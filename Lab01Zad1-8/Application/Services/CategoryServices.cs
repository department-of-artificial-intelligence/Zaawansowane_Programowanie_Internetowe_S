using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
using Application.Domain;

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
            .EnsureUnique(category.Name, category.UserId)
            .Map(_ => category)
        )
        .Tap(categoryRepository.Add)
        .Tap(_ => unitOfWork.Save())
        .Map(AddCategoryResult.FromDomain);

    public Result<Unit> RemoveCategory(Guid id)
        => categoryRepository.GetById(id)
            .Required($"Nie mogę odnaleźć kategorii o id = {id}")
            .Bind(category =>
                contactRepository.IsCategoryUsed(id)
                ? Result.Failure<Category>("Nie można usunąć kategorii ponieważ są do niej przypisane adresy e-mail")
                : Result.Success(category))
            .Tap(categoryRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> UpdateCategory(UpdateCategoryDto dto)
        => categoryRepository.GetById(dto.Id)
            .Required($"Nie mogę odnaleźć kategorii o id = {dto.Id}")
            .Bind(category => category.Rename(dto.NewName))
            .Tap(_ => unitOfWork.Save());
}