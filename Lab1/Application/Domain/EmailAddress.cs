using System;
using System.Text.RegularExpressions;

namespace Application.Domain
{
    public record EmailAddress
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Address { get; }

        public EmailAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address) || !EmailRegex.IsMatch(address))
            {
                throw new ArgumentException($"'{address}' nie jest poprawnym adresem e-mail.", nameof(address));
            }

            Address = address;
        }

        public static implicit operator string(EmailAddress address) => address.Address;
        public static implicit operator EmailAddress(string text) => new EmailAddress(text);
    }
}