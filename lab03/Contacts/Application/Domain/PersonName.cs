using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

// Zadanie 1 - obiekty wartości dla imienia i nazwiska.
public sealed record FirstName
{
    public string Value { get; }

    private FirstName(string value)
    {
        Value = value;
    }

    public static Result<FirstName> Create(string value) =>
        PersonName
            .Validate(value, "Imię")
            .Map(name => new FirstName(name));
}

public sealed record LastName
{
    public string Value { get; }

    private LastName(string value)
    {
        Value = value;
    }

    public static Result<LastName> Create(string value) =>
        PersonName
            .Validate(value, "Nazwisko")
            .Map(name => new LastName(name));
}

// Wspólne reguły walidacji imienia i nazwiska.
internal static partial class PersonName
{
    public const int MaxLength = 50;

    // Litery (również polskie), opcjonalnie rozdzielone spacją, myślnikiem lub apostrofem,
    // np. "Anna Maria", "Kowalska-Nowak", "O'Connor".
    [GeneratedRegex(@"^\p{L}+([ '\-]\p{L}+)*$")]
    private static partial Regex Pattern();

    public static Result<string> Validate(string value, string fieldName) =>
        Validators
            .NotEmpty(value, $"{fieldName} jest wymagane.")
            .Map(_ => value.Trim())
            .Bind(trimmed => Result.Combine(
                    Validators.MaxLength(trimmed, MaxLength,
                        $"{fieldName} może mieć maksymalnie {MaxLength} znaków."),
                    Validators.Matches(trimmed, Pattern(),
                        $"{fieldName} może zawierać tylko litery, spacje, myślniki i apostrofy."))
                .Map(_ => trimmed));
}
