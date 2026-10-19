using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed record PhoneNumber
{
    private static readonly Regex PhonePattern = new(
        @"^\+?\d{9,15}$",
        RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string value) =>
        Validators.NotEmpty(value, "Numer telefonu jest wymagany.")
            .Map(_ => Normalize(value))
            .Bind(normalized => Validators
                .Matches(normalized, PhonePattern,
                    $"'{value}' nie jest poprawnym numerem telefonu.")
                .Map(_ => new PhoneNumber(normalized)));

    // Usuwa spacje, myślniki i nawiasy: "+48 (600) 100-200" -> "+48600100200"
    private static string Normalize(string value) =>
        new(value
            .Where(ch => !char.IsWhiteSpace(ch) && ch is not '-' and not '(' and not ')')
            .ToArray());
}