using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed partial record EmailAddress
{
    public string Value {get;}

    private EmailAddress(string value)
    {
        Value = value;
    }

    // Część lokalna, znak @, domena zawierająca co najmniej jedną kropkę
    // i domena najwyższego poziomu złożona z co najmniej dwóch liter (zadanie 2).
    [GeneratedRegex(@"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9\-]+(\.[A-Za-z0-9\-]+)*\.[A-Za-z]{2,}$")]
    private static partial Regex Pattern();

    public static Result<EmailAddress> Create(string value) =>
        Validators
            .NotEmpty(value, "Adres email jest wymagany.")
            .Map(_ => value.Trim())
            .Bind(trimmed => Validators
                .Matches(trimmed, Pattern(), $"Adres email '{trimmed}' ma niepoprawny format.")
                .Map(_ => new EmailAddress(trimmed)));
}
