using System.Text.RegularExpressions;
using Application.Utilities;
namespace Application.Domain;

public sealed record PhoneNumber
{
    public const int MaxChars = 16;

    // opcjonalny "+" i 9–15 cyfr (format E.164)
    private static readonly Regex Pattern = new(@"^\+?[0-9]{9,15}$", RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber> Create(string value) =>
        Validators.NotEmpty(value, "Numer telefonu jest wymagany.")
            .Map(_ => Normalize(value))
            .Bind(normalized => Validators
                .Matches(normalized, Pattern, "Niepoprawny numer telefonu.")
                .Map(_ => new PhoneNumber(normalized)));

    // "+48 600-100-200" -> "+48600100200"
    private static string Normalize(string value) =>
        new(value.Where(c => !char.IsWhiteSpace(c) && c is not ('-' or '(' or ')')).ToArray());

    public override string ToString() => Value;
}