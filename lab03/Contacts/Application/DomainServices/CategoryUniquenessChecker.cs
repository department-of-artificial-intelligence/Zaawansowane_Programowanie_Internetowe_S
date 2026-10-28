using Application.Repositories;
using Application.Utilities;

namespace Application.DomainServices;

// Nazwa kategorii jest unikalna w obrębie kategorii jednego użytkownika.
public class CategoryUniquenessChecker(ICategoryRepository categoryRepository)
{
    public Result<Unit> EnsureUnique(string name, Guid userId) =>
        categoryRepository.GetByNameAndUser(name, userId).Match(
            _ => Result.Failure<Unit>(
                $"Kategoria o nazwie '{name}' już istnieje."
            ),
            () => Result.Success(Unit.Instance));

    // Wariant dla zmiany nazwy - kategoria może zachować swoją dotychczasową nazwę.
    public Result<Unit> EnsureUnique(string name, Guid userId, Guid renamedCategoryId) =>
        categoryRepository.GetByNameAndUser(name, userId).Match(
            existing => existing.Id == renamedCategoryId
                ? Result.Success(Unit.Instance)
                : Result.Failure<Unit>($"Kategoria o nazwie '{name}' już istnieje."),
            () => Result.Success(Unit.Instance));
}
