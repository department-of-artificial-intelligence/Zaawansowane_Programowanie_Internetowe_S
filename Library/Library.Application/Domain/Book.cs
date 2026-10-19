using Application.Utilities;

namespace Library.Application.Domain;

public class Book : Entity
{
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public Isbn Isbn { get; private set; } = null!;
    public bool IsBorrowed { get; private set; }

    private Book() : base(Guid.Empty) { }

    private Book(Guid id, string title, string author, Isbn isbn) : base(id)
    {
        Title = title;
        Author = author;
        Isbn = isbn;
        IsBorrowed = false;
    }

    public static Result<Book> Create(string title, string author, Isbn isbn) =>
        Result.Combine(
            Validators.NotEmpty(title, "Tytuł książki jest wymagany."),
            Validators.NotEmpty(author, "Autor książki jest wymagany.")
        ).Map(_ => new Book(Guid.NewGuid(), title.Trim(), author.Trim(), isbn));

    public Result<Unit> MarkAsBorrowed()
    {
        if (IsBorrowed)
            return Result.Failure<Unit>("Książka jest już wypożyczona.");

        IsBorrowed = true;
        return Result.Success(Unit.Instance);
    }

    public Result<Unit> MarkAsReturned()
    {
        if (!IsBorrowed)
            return Result.Failure<Unit>("Książka nie była oznaczona jako wypożyczona.");

        IsBorrowed = false;
        return Result.Success(Unit.Instance);
    }
}