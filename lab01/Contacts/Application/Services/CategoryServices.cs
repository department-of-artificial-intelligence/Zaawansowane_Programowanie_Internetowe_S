using Application.Domain;
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

    // Zadanie 3 - modyfikacja (zmiana nazwy) kategorii.
    public Result<Unit> RenameCategory(Guid id, string name)
        => FindCategory(id)
            .Bind(category =>
                categoryUniquenessChecker
                    .EnsureUnique(name?.Trim() ?? "", category.Id)
                    .Bind(_ => category.Rename(name!)))
            .Tap(_ => unitOfWork.Save());

    // Zadanie 4 - adres email musi mieć zawsze przypisaną kategorię,
    // więc nie można usunąć kategorii, która jest jeszcze używana.
    public Result<Unit> RemoveCategory(Guid id)
        => FindCategory(id)
            .Bind(category => contactRepository.IsCategoryInUse(category.Id)
                ? Result.Failure<Category>(
                    "Nie można usunąć kategorii, do której są przypisane adresy email.")
                : Result.Success(category))
            .Tap(categoryRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    private Result<Category> FindCategory(Guid id)
        => categoryRepository
            .GetById(id)
            .Required($"Nie mogę odnaleźć kategorii o id = {id}");
}
