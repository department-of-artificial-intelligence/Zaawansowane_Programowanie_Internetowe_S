using Application.Utilities;

namespace Application.Domain;

public class Reader
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    
    private readonly List<Guid> _borrowedBooks = [];
    public IReadOnlyCollection<Guid> BorrowedBooks => _borrowedBooks;

    private Reader() { Email = null!; }

    private Reader(string email)
    {
        Id = Guid.NewGuid();
        Email = email;
    }

    public static Result<Reader> Create(string email) =>
        Result.Success(new Reader(email));

    public Result<Unit> BorrowBook(Guid bookId)
    {
        if (_borrowedBooks.Count >= 2)
            return Result.Failure<Unit>("Przekroczono limit 2 wypożyczonych książek");

        if (_borrowedBooks.Contains(bookId))
            return Result.Failure<Unit>("Czytelnik już posiada tę książkę");

        _borrowedBooks.Add(bookId);
        return Result.Success(Unit.Instance);
    }
}