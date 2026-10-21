using Application.Repositories;
using Application.Utilities;

namespace Application.DomainServices;

public class CategoryUniquenessChecker(ICategoryRepository categoryRepository)
{
    public Result<Unit> EnsureUnique(string name) =>
        categoryRepository.GetByName(name).Match(
            _ => Result.Failure<Unit>(
                $"Kategoria o nazwie '{name}' już istnieje."
            ),
            () => Result.Success(Unit.Instance));

    // Wariant dla zmiany nazwy (zadanie 3) - kategoria może zachować swoją dotychczasową nazwę.
    public Result<Unit> EnsureUnique(string name, Guid renamedCategoryId) =>
        categoryRepository.GetByName(name).Match(
            existing => existing.Id == renamedCategoryId
                ? Result.Success(Unit.Instance)
                : Result.Failure<Unit>($"Kategoria o nazwie '{name}' już istnieje."),
            () => Result.Success(Unit.Instance));
}
