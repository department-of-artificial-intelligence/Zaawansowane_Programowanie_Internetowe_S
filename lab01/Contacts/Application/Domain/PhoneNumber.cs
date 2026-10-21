using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed partial record PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    // Opcjonalny prefiks "+" i od 9 do 15 cyfr (format E.164).
    [GeneratedRegex(@"^\+?[0-9]{9,15}$")]
    private static partial Regex Pattern();

    // Spacje, myślniki i nawiasy są usuwane, więc "+48 123-456-789"
    // i "+48123456789" to ten sam numer.
    [GeneratedRegex(@"[\s\-()]")]
    private static partial Regex Separators();

    public static Result<PhoneNumber> Create(string value) =>
        Validators
            .NotEmpty(value, "Numer telefonu jest wymagany.")
            .Map(_ => Separators().Replace(value, ""))
            .Bind(normalized => Validators
                .Matches(normalized, Pattern(), $"Numer telefonu '{value}' ma niepoprawny format.")
                .Map(_ => new PhoneNumber(normalized)));
}
