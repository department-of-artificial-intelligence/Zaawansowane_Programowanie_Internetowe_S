
using Application.Utilities;
using System.Text.RegularExpressions;

namespace Application.Domain;

public sealed record PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !Regex.IsMatch(value, @"^\+?[0-9\s()\-]+$") ||
            Regex.Replace(value, @"\D", "").Length is < 9 or > 15)
        {
            return Result.Failure<PhoneNumber>(
                "Numer telefonu ma niepoprawny format.");
        }

        return Result.Success(
            new PhoneNumber(value.Trim()));
    }
}
