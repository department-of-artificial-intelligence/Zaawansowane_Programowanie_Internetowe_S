using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed record PhoneNumber
{
    private static readonly Regex Format =
        new(@"^\+?[0-9][0-9\s-]{5,19}$", RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber> Create(string value) =>
        !string.IsNullOrWhiteSpace(value) && Format.IsMatch(value.Trim())
            ? Result.Success(new PhoneNumber(value.Trim()))
            : Result.Failure<PhoneNumber>("Numer telefonu ma niepoprawny format.");
}
