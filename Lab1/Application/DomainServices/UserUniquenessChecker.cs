using Application.Repositories;
using Application.Utilities;

namespace Application.DomainServices;

public class UserUniquenessChecker(IUserRepository userRepository)
{
    public Result<Unit> EnsureUnique(string userName) =>
        userRepository.GetByName(userName).Match(
            _ => Result.Failure<Unit>($"Login '{userName}' jest już zajęty."),
            () => Result.Success(Unit.Instance));
}
