using Application.Utilities;

namespace Application.Domain;

public enum BookStatus { Available, Borrowed }

public class Book
{
    public Guid Id { get; private set; }
    public Isbn Isbn { get; private set; }
    public string Title { get; private set; }
    public BookStatus Status { get; private set; }

    private Book() { Isbn = null!; Title = null!; }

    private Book(Isbn isbn, string title)
    {
        Id = Guid.NewGuid();
        Isbn = isbn;
        Title = title;
        Status = BookStatus.Available;
    }

    public static Result<Book> Create(Isbn isbn, string title) =>
        Result.Success(new Book(isbn, title));

    public Result<Unit> Borrow()
    {
        if (Status == BookStatus.Borrowed)
            return Result.Failure<Unit>("Książka jest już wypożyczona.");
            
        Status = BookStatus.Borrowed;
        return Result.Success(Unit.Instance);
    }
}