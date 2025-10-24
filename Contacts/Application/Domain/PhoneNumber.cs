using System.Text.RegularExpressions;

namespace Contacts.Application.Domain;

public record PhoneNumber
{
    public string Number { get; private set; } = string.Empty;

    // EF Core
    private PhoneNumber() { }

    public PhoneNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Numer telefonu nie może być pusty", nameof(number));

        var trimmed = number.Trim();
        // Prosta normalizacja: usuń spacje, myślniki, nawiasy
        var normalized = Regex.Replace(trimmed, @"[\n\r\t\s\-()]+", "");
        // Pozwól na opcjonalny '+' na początku, reszta cyfry
        var pattern = @"^\+?[0-9]{6,15}$";
        if (!Regex.IsMatch(normalized, pattern))
            throw new ArgumentException("Nieprawidłowy format numeru telefonu", nameof(number));

        Number = normalized;
    }

    public static implicit operator string(PhoneNumber phone) => phone.Number;
    public static implicit operator PhoneNumber(string text) => new(text);
}