using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Application.DomainServices;

public class ReaderUniquenessChecker(IReaderRepository readerRepository)
{
    public Result<Unit> EnsureUnique(EmailAddress email) =>
        readerRepository.GetByEmail(email).Match(
            _ => Result.Failure<Unit>(
                $"Czytelnik z adresem {email.Value} już istnieje."),
            () => Result.Success(Unit.Instance));
}
