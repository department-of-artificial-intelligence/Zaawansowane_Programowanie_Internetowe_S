using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed record Isbn
{
    private static readonly Regex Pattern = new(@"^(\d{10}|\d{13})$");

    public string Value { get; }

    private Isbn(string value)
    {
        Value = value;
    }

    public static Result<Isbn> Create(string value) =>
        Pattern.IsMatch(value)
            ? Result.Success(new Isbn(value))
            : Result.Failure<Isbn>("Numer ISBN musi składać się z 10 lub 13 cyfr.");
}
