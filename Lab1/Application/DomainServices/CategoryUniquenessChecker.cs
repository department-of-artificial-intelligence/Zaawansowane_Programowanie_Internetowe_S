using Application.Repositories;
using Application.Utilities;
namespace Application.DomainServices;

public class CategoryUniquenessChecker(ICategoryRepository categoryRepository)
{
    // exceptId pozwala zmienić nazwę kategorii "na tę samą" (zadanie 3, Lab1)
    public Result<Unit> EnsureUnique(string name, Guid? exceptId = null) =>
        categoryRepository.GetByName(name).Match(
            existing => existing.Id == exceptId
                ? Result.Success(Unit.Instance)
                : Result.Conflict<Unit>($"Kategoria o nazwie '{name}' już istnieje."),
            () => Result.Success(Unit.Instance));
}
