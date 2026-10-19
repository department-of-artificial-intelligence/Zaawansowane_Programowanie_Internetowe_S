using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed record EmailAddress
{
    private static readonly Regex EmailPattern = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled);

    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static Result<EmailAddress> Create(string value) =>
        Validators.NotEmpty(value, "Adres e-mail jest wymagany.")
            .Bind(_ => Validators.Matches(
                value.Trim(),
                EmailPattern,
                $"'{value}' nie jest poprawnym adresem e-mail."))
            .Map(_ => new EmailAddress(value.Trim().ToLowerInvariant()));
}