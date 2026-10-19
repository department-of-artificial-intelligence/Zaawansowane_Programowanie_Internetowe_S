using Application.Utilities;
using System.Text.RegularExpressions;

namespace Application.Domain;

public record EmailAddress
{
    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static Result<EmailAddress> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<EmailAddress>(
                "Adres e-mail nie może być pusty.");

        value = value.Trim();

        if (!Regex.IsMatch(
            value,
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)+$"))
        {
            return Result.Failure<EmailAddress>(
                "Niepoprawny format adresu e-mail.");
        }

        return Result.Success(new EmailAddress(value));
    }
}
