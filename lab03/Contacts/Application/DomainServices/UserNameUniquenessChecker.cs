using Application.Repositories;
using Application.Utilities;

namespace Application.DomainServices;

// Zadanie 2 - sprawdzanie unikalności loginu (na wzór unikalności kategorii).
public class UserNameUniquenessChecker(IUserRepository userRepository)
{
    public Result<Unit> EnsureUnique(string userName) =>
        userRepository.GetByName(userName).Match(
            _ => Result.Failure<Unit>(
                $"Użytkownik o nazwie '{userName}' już istnieje."
            ),
            () => Result.Success(Unit.Instance));
}
