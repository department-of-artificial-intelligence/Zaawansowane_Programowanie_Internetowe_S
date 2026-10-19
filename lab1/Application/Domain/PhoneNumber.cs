
using Application.Utilities;
using System.Text.RegularExpressions;

namespace Application.Domain;

public record PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<PhoneNumber>(
                "Numer telefonu nie może być pusty.");

        value = value.Trim();

        if (!Regex.IsMatch(value, @"^\+?[1-9]\d{8,14}$"))
            return Result.Failure<PhoneNumber>(
                "Niepoprawny format numeru telefonu.");

        return Result.Success(new PhoneNumber(value));
    }
}
