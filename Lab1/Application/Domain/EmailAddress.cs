using System.Text.RegularExpressions;
using Application.Utilities;
namespace Application.Domain;

public sealed record EmailAddress
{
    public const int MaxChars = 254;

    private static readonly Regex Pattern = new(
        @"^[a-z0-9._%+-]+@[a-z0-9-]+(\.[a-z0-9-]+)*\.[a-z]{2,}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static Result<EmailAddress> Create(string value) =>
        Validators.NotEmpty(value, "Adres e-mail jest wymagany.")
            .Map(_ => value.Trim().ToLowerInvariant())
            .Bind(normalized =>
                Validators.MaxLength(normalized, MaxChars, "Adres e-mail jest za długi.")
                    .Bind(_ => Validators.Matches(normalized, Pattern,
                        "Niepoprawny format adresu e-mail."))
                    .Map(_ => new EmailAddress(normalized)));

    public override string ToString() => Value;
}