using Application.Repositories;
using Application.Utilities;

namespace Application.DomainServices;

public class CategoryUsageChecker(IContactRepository contactRepository)
{
    public Result<Unit> EnsureNotUsed(Guid categoryId) =>
        contactRepository.AnyEmailInCategory(categoryId)
            ? Result.Failure<Unit>(
                "Nie można usunąć kategorii, ponieważ jest przypisana do co najmniej jednego adresu e-mail.")
            : Result.Success(Unit.Instance);
}