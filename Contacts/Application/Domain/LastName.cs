using System.Text.RegularExpressions;

namespace Contacts.Application.Domain;

public record LastName
{
    public string Value { get; }

    public LastName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Nazwisko nie może być puste", nameof(value));

        var trimmed = value.Trim();
        if (trimmed.Length < 2 || trimmed.Length > 80)
            throw new ArgumentOutOfRangeException(nameof(value), "Nazwisko musi mieć od 2 do 80 znaków");

        // Litery (w tym akcentowane), spacje, myślniki i apostrofy
        var pattern = "^[A-Za-zÀ-ÖØ-öø-ÿ' -]+$";
        if (!Regex.IsMatch(trimmed, pattern))
            throw new ArgumentException("Nazwisko zawiera niedozwolone znaki", nameof(value));

        Value = trimmed;
    }

    public static implicit operator string(LastName name) => name.Value;
    public static implicit operator LastName(string value) => new(value);
}