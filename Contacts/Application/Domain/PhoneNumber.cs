using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed record PhoneNumber
{
    private static readonly Regex PhoneRegex = new(
        @"^(\+?[0-9]{1,3})?[0-9]{9,11}$",
        RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<PhoneNumber>("Numer telefonu nie może być pusty.");
        }

        var normalized = Regex.Replace(value.Trim(), @"[\s\-]", "");
        if (!PhoneRegex.IsMatch(normalized))
        {
            return Result.Failure<PhoneNumber>("Niepoprawny format numeru telefonu.");
        }

        return Result.Success(new PhoneNumber(normalized));
    }

    public static implicit operator string(PhoneNumber phone) => phone.Value;
}