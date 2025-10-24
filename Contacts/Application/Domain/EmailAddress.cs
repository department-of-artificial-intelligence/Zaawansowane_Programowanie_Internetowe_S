using System.Text.RegularExpressions;

namespace Contacts.Application.Domain;

public record EmailAddress
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled);

    // POPRAWKA: Dodajemy '= null!', aby usunąć ostrzeżenie CS8618
    public string Address { get; private set; } = null!;

    // Konstruktor publiczny (dla logiki biznesowej)
    public EmailAddress(string address) 
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentNullException(nameof(address), "Adres email nie może być pusty.");
        }
        if (!EmailRegex.IsMatch(address))
        {
            throw new ArgumentException("Niepoprawny format adresu email.", nameof(address));
        }
        
        this.Address = address; 
    }

    // Prywatny konstruktor bezparametrowy (dla EF Core)
    private EmailAddress() {} 

    // Operatory konwersji
    public static implicit operator string(EmailAddress address) => address.Address;
    public static implicit operator EmailAddress(string text) => new(text);
}