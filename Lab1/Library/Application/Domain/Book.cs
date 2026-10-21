using Application.Utilities;

namespace Application.Domain;

public class Book : Entity
{
    private const int MaxCopies = 100;

    public string Title { get; private set; }
    public string Author { get; private set; }
    public Isbn Isbn { get; private set; }
    public int TotalCopies { get; private set; }
    public int AvailableCopies { get; private set; }

    private Book(
        Guid id,
        string title,
        string author,
        Isbn isbn,
        int totalCopies) : base(id)
    {
        Title = title;
        Author = author;
        Isbn = isbn;
        TotalCopies = totalCopies;
        AvailableCopies = totalCopies;
    }

    public static Result<Book> Create(
        string title,
        string author,
        Isbn isbn,
        int totalCopies) =>
        Result.Combine(
            Validators.NotEmpty(title, "Tytuł jest wymagany."),
            Validators.NotEmpty(author, "Autor jest wymagany."),
            Validators.InRange(
                totalCopies,
                1,
                MaxCopies,
                $"Liczba egzemplarzy musi mieścić się w zakresie 1-{MaxCopies}.")
        ).Map(_ => new Book(Guid.NewGuid(), title, author, isbn, totalCopies));

    public Result<Unit> Update(string title, string author, int totalCopies)
    {
        var borrowed = TotalCopies - AvailableCopies;
        var minCopies = Math.Max(1, borrowed);

        return Result.Combine(
            Validators.NotEmpty(title, "Tytuł jest wymagany."),
            Validators.NotEmpty(author, "Autor jest wymagany."),
            Validators.InRange(
                totalCopies,
                minCopies,
                MaxCopies,
                $"Liczba egzemplarzy musi mieścić się w zakresie {minCopies}-{MaxCopies}.")
        ).Tap(_ =>
        {
            Title = title;
            Author = author;
            TotalCopies = totalCopies;
            AvailableCopies = totalCopies - borrowed;
        });
    }

    public Result<Unit> TakeCopy()
    {
        if (AvailableCopies == 0)
        {
            return Result.Failure<Unit>("Brak wolnych egzemplarzy.");
        }

        AvailableCopies--;
        return Result.Success(Unit.Instance);
    }

    public Result<Unit> ReturnCopy()
    {
        if (AvailableCopies == TotalCopies)
        {
            return Result.Failure<Unit>("Wszystkie egzemplarze są już zwrócone.");
        }

        AvailableCopies++;
        return Result.Success(Unit.Instance);
    }
}
