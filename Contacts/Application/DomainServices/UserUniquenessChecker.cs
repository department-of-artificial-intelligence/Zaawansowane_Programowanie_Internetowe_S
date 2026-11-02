using Application.Repositories;
using Application.Utilities;

namespace Application.DomainServices;

public class UserUniquenessChecker(IUserRepository userRepository)
{
    public Result<Unit> EnsureUnique(string userName) =>
        userRepository.GetByName(userName).Match(
            _ => Result.Failure<Unit>(
                $"Użytkownik o loginie '{userName}' już istnieje."),
            () => Result.Success(Unit.Instance));
}