using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Application.Domain
{
    public sealed record EmailAddress
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        private EmailAddress(string value)
        {
            Value = value;
        }

        public static Result<EmailAddress> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<EmailAddress>("Adres e-mail nie może być pusty.");
            }

            if (!EmailRegex.IsMatch(value))
            {
                return Result.Failure<EmailAddress>($"Niepoprawny format adresu e-mail: '{value}'.");
            }

            return Result.Success(new EmailAddress(value));
        }
    }

}
