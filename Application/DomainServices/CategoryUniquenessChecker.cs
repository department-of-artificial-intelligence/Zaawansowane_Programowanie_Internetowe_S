using Application.Repositories;
using Application.Utilities;
namespace Application.DomainServices;

public class CategoryUniquenessChecker(ICategoryRepository categoryRepository)
{
    public Result<Unit> EnsureUnique(string name, Guid userId) =>
        categoryRepository.GetByNameAndUser(name, userId).Match(
            _ => Result.Failure<Unit>(
                $"Kategoria o nazwie '{name}' już istnieje."),
            () => Result.Success(Unit.Instance));

    // do zmiany nazwy – pomija zmienianą kategorię
    public Result<Unit> EnsureUnique(string name, Guid userId, Guid excludedCategoryId) =>
        categoryRepository.GetByNameAndUser(name, userId).Match(
            existing => existing.Id == excludedCategoryId
                ? Result.Success(Unit.Instance)
                : Result.Failure<Unit>(
                    $"Kategoria o nazwie '{name}' już istnieje."),
            () => Result.Success(Unit.Instance));
}