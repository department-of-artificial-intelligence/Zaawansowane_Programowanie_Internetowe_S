using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed record EmailAddress
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static Result<EmailAddress> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<EmailAddress>("Adres email nie może być pusty.");
        }

        var trimmed = value.Trim();
        if (!EmailRegex.IsMatch(trimmed))
        {
            return Result.Failure<EmailAddress>("Niepoprawny format adresu email.");
        }

        return Result.Success(new EmailAddress(trimmed));
    }

    public static implicit operator string(EmailAddress email) => email.Value;
}