using Application.Utilities;

namespace Application.Domain;

public sealed record BookTitle
{
    public string Value { get; }

    private BookTitle(string value)
    {
        Value = value;
    }
    private BookTitle() 
    { 
        Value = null!; 
    }
    public static Result<BookTitle> Create(string value) =>
        Validators
            .NotEmpty(value, "Nazwa ksiazki nie może być pusta.")
            .Map(_ => new BookTitle(value));
}