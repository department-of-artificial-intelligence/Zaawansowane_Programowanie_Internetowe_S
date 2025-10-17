using System.Text.RegularExpressions;

namespace Contacts.Application.Domain;

public record EmailAddress
{
    public string Address { get; }

    private const string EmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    public EmailAddress() : this(string.Empty) { }
    public EmailAddress(string Address)
    {
        if (string.IsNullOrWhiteSpace(Address))
        {
            throw new ArgumentException("Adres email nie może być pusty.", nameof(Address));
        }

        if (!Regex.IsMatch(Address, EmailRegex))
        {
            throw new ArgumentException($"Niepoprawny format adresu email: '{Address}'.", nameof(Address));
        }

        this.Address = Address;
    }

    public static implicit operator string(EmailAddress address)
    => address.Address;
    public static implicit operator EmailAddress(string address)
    => new(address);
}