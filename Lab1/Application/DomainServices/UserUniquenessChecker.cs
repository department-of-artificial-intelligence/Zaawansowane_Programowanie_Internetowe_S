using Application.Repositories;
using Application.Utilities;
namespace Application.DomainServices;

// Zadanie 2: na wzór CategoryUniquenessChecker
public class UserUniquenessChecker(IUserRepository userRepository)
{
    public Result<Unit> EnsureUnique(string userName) =>
        userRepository
            .GetByName(userName)
            .Match(
                _ => Result.Conflict<Unit>($"Użytkownik o nazwie {userName} już istnieje"),
                () => Result.Success(Unit.Instance));
}
