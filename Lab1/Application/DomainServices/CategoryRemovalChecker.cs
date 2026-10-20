using Application.Domain;
using Application.Repositories;
using Application.Utilities;
namespace Application.DomainServices;

public class CategoryRemovalChecker(IContactRepository contactRepository)
{
    public Result<Unit> EnsureCanBeRemoved(Category category) =>
        Validators.IsTrue(
            !contactRepository.IsCategoryInUse(category.Id),
            $"Kategoria '{category.Name}' jest przypisana do adresów e-mail " +
            "i nie może zostać usunięta.");
}