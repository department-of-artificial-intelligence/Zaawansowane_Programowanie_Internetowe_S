using Application.Utilities;
using System.Text.RegularExpressions;

namespace Application.Domain;

public sealed record EmailAddress
{
    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    public static Result<EmailAddress> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<EmailAddress>("Adres e-mail nie może być pusty.");
        }

        if (!EmailRegex.IsMatch(value))
        {
            return Result.Failure<EmailAddress>("Niepoprawny format adresu e-mail.");
        }

        return Result.Success(new EmailAddress(value));
    }
}