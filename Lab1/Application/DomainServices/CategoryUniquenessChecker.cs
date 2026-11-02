using Application.Repositories;
using Application.Utilities;
namespace Application.DomainServices;

public class CategoryUniquenessChecker(ICategoryRepository categoryRepository)
{
    // 3.27: nazwa musi być unikalna w obrębie użytkownika.
    // exceptId pozwala zmienić nazwę kategorii "na tę samą" (zmiana nazwy z Lab1).
    public Result<Unit> EnsureUnique(string name, Guid userId, Guid? exceptId = null) =>
        categoryRepository
            .GetByNameAndUser(name, userId)
            .Match(
                existing => existing.Id == exceptId
                    ? Result.Success(Unit.Instance)
                    : Result.Conflict<Unit>($"Kategoria o nazwie {name} już istnieje"),
                () => Result.Success(Unit.Instance));
}
