using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Application.DomainServices;

public class BookUniquenessChecker(IBookRepository bookRepository)
{
    public Result<Unit> EnsureUnique(Isbn isbn) =>
        bookRepository.GetByIsbn(isbn).Match(
            _ => Result.Failure<Unit>(
                $"Książka o numerze ISBN {isbn.Value} już istnieje w katalogu."),
            () => Result.Success(Unit.Instance));
}
