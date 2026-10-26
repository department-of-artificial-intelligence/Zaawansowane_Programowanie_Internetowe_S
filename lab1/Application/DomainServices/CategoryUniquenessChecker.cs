
using Application.Repositories;
using Application.Utilities;

namespace Application.DomainServices;

public class CategoryUniquenessChecker(
    ICategoryRepository categoryRepository)
{
    public Result<Unit> EnsureUnique(string name) =>
        categoryRepository.GetByName(name).Match(
            _ => Result.Failure<Unit>(
                $"Kategoria o nazwie '{name}' już istnieje."
            ),
            () => Result.Success(Unit.Instance)
        );

    public Result<Unit> EnsureUnique(
        string name,
        Guid excludedCategoryId) =>
        categoryRepository.GetByName(name).Match(
            category => category.Id == excludedCategoryId
                ? Result.Success(Unit.Instance)
                : Result.Failure<Unit>(
                    $"Kategoria o nazwie '{name}' już istnieje."
                ),
            () => Result.Success(Unit.Instance)
        );
}
