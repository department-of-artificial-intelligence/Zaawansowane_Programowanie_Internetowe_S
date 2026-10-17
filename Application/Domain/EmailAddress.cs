using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Domain;

public sealed record EmailAddress
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(250)
    );
    public string Value { get; }
    private EmailAddress(string value)
    {
        Value = value;
    }
    public static Result<EmailAddress> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<EmailAddress>("Adres email jest wymagany.");
        }

        if (!EmailRegex.IsMatch(value))
        {
            return Result.Failure<EmailAddress>("Niepoprawny format adresu email.");
        }

        return Result.Success(new EmailAddress(value.Trim()));
    }
}
