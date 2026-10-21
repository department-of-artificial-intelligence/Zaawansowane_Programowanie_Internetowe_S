using Application.Utilities;

namespace Application.Domain;

public sealed record Isbn
{
    public string Value { get; }

    private Isbn(string value) => Value = value;

    public static Result<Isbn> Create(string value) =>
        Validators.NotEmpty(value, "ISBN nie może być pusty")
            .Map(_ => new Isbn(value));
}