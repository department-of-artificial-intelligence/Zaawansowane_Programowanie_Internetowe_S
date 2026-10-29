using Application.Utilities;

namespace Application.Domain;

public record ISBN
{
    public string Value { get; private set; } = null!;

    private ISBN() { }

    private ISBN(string value)
    {
        Value = value;
    }

    public static Result<ISBN> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<ISBN>("Numer ISBN nie może być pusty.");
        }

        var cleaned = value.Replace("-", "").Replace(" ", "");
        if (cleaned.Length != 10 && cleaned.Length != 13)
        {
            return Result.Failure<ISBN>("ISBN musi posiadać 10 lub 13 cyfr.");
        }

        return Result.Success(new ISBN(value.Trim()));
    }

    public override string ToString() => Value;
}