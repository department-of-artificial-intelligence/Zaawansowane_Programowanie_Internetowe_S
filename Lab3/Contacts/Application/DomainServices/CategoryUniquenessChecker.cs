using Application.Repositories;
using Application.Utilities;

namespace Application.DomainServices;

public class CategoryUniquenessChecker(ICategoryRepository categoryRepository)
{
    public Result<Unit> EnsureUnique(string name, Guid userId) =>
        categoryRepository.GetByNameAndUser(name, userId).Match(
            _ => Result.Failure<Unit>(
                $"Kategoria o nazwie '{name}' już istnieje."
            ),
            () => Result.Success(Unit.Instance));
}
