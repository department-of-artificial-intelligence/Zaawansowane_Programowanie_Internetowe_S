
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

    public static Result<EmailAddress> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return Result.Failure<EmailAddress>(
                "Niepoprawny format adresu email.");
        }

        return Result.Success(new EmailAddress(value));
    }
}
