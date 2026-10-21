using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed record EmailAddress
{
    private static readonly Regex Pattern = new(
        @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$");

    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static Result<EmailAddress> Create(string value) =>
        Pattern.IsMatch(value)
            ? Result.Success(new EmailAddress(value))
            : Result.Failure<EmailAddress>("Adres email ma niepoprawny format.");
}
