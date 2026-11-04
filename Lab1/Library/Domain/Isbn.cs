using Application.Utilities;

namespace Library.Domain;

public sealed record Isbn
{
    public string Value { get; }
    private Isbn(string value) => Value = value;

    public static Result<Isbn> Create(string value)
    {
        var normalized = value?.Replace("-", "").Replace(" ", "");
        return normalized is not null && normalized.Length is 10 or 13 &&
               normalized.All(char.IsDigit)
            ? Result.Success(new Isbn(normalized))
            : Result.Failure<Isbn>("ISBN musi zawierać 10 lub 13 cyfr.");
    }
}
