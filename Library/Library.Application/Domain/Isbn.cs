using System.Text.RegularExpressions;
using Application.Utilities;

namespace Library.Application.Domain;

public sealed record Isbn
{
    private static readonly Regex IsbnRegex = new(@"^(97[89])?[0-9]{9}[0-9X]$", RegexOptions.Compiled);

    public string Value { get; }

    private Isbn(string value) => Value = value;

    public static Result<Isbn> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Isbn>("ISBN nie może być pusty.");

        var clean = Regex.Replace(value.Trim(), @"[\s\-]", "");
        if (!IsbnRegex.IsMatch(clean))
            return Result.Failure<Isbn>("Niepoprawny format ISBN.");

        return Result.Success(new Isbn(clean));
    }

    public static implicit operator string(Isbn isbn) => isbn.Value;
}