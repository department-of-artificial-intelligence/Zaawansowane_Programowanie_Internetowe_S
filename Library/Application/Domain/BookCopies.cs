using Application.Utilities;

namespace Application.Domain;

public sealed record BookCopies
{
    public int Value { get; }

    private BookCopies(int value)
    {
        Value = value;
    }

    public static Result<BookCopies> Create(int value) =>
        Validators
            .InRange(value, 0, "Liczba ksiazek nie moze byc ujemna")
            .Map(_ => new BookCopies(value));
}