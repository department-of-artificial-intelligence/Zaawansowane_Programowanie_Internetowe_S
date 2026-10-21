using Application.Utilities;

namespace Application.Domain;

public sealed record Isbn
{
    public string Value { get; }

    private Isbn(string value)
    {
        Value = value;
    }

    // Akceptuje ISBN-10 i ISBN-13 (z myślnikami lub bez) i sprawdza cyfrę kontrolną.
    public static Result<Isbn> Create(string value) =>
        Validators
            .NotEmpty(value, "ISBN jest wymagany.")
            .Map(_ => value.Replace("-", "").Replace(" ", "").ToUpperInvariant())
            .Bind(normalized => IsValid(normalized)
                ? Result.Success(new Isbn(normalized))
                : Result.Failure<Isbn>($"ISBN '{value}' jest niepoprawny."));

    private static bool IsValid(string isbn) => isbn.Length switch
    {
        10 => IsValidIsbn10(isbn),
        13 => IsValidIsbn13(isbn),
        _ => false
    };

    private static bool IsValidIsbn10(string isbn)
    {
        if (!isbn[..9].All(char.IsAsciiDigit)) return false;
        if (!char.IsAsciiDigit(isbn[9]) && isbn[9] != 'X') return false;

        var sum = 0;
        for (var i = 0; i < 10; i++)
        {
            var digit = isbn[i] == 'X' ? 10 : isbn[i] - '0';
            sum += (10 - i) * digit;
        }
        return sum % 11 == 0;
    }

    private static bool IsValidIsbn13(string isbn)
    {
        if (!isbn.All(char.IsAsciiDigit)) return false;

        var sum = 0;
        for (var i = 0; i < 13; i++)
        {
            sum += (isbn[i] - '0') * (i % 2 == 0 ? 1 : 3);
        }
        return sum % 10 == 0;
    }
}
