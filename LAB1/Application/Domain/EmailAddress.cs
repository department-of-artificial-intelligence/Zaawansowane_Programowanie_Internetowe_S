using System.Text.RegularExpressions;

namespace Contacts.Application.Domain;

public record EmailAddress
{
    public string Address { get; }
    public EmailAddress(string address)
    {
        ValidateEmail(address);
        Address = address;
    }

    private static void ValidateEmail(string address)
    {
        string wzor = @"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$";
        if (!Regex.IsMatch(address, wzor))
        {
            throw new ArgumentException("Niepoprawny adres email");
        }
    }
    
    public static implicit operator string(EmailAddress address) => address.Address;
    public static implicit operator EmailAddress(string text) => new(text);
}