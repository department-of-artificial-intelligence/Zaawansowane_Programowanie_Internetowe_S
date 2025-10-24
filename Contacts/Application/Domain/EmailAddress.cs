using System.Text.RegularExpressions;

namespace Contacts.Application.Domain;

public record EmailAddress
{
    public string Address { get; private set; } = string.Empty;

    // Dla EF Core
    private EmailAddress() { }

    public EmailAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Adres email nie może być pusty", nameof(address));

        var trimmed = address.Trim();

        // Prosta, praktyczna walidacja formatu email 
        var pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
        if (!Regex.IsMatch(trimmed, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            throw new ArgumentException("Nieprawidłowy format adresu email", nameof(address));

        Address = trimmed;
    }

    public static implicit operator string(EmailAddress address) => address.Address;
    public static implicit operator EmailAddress(string text) => new(text);
}