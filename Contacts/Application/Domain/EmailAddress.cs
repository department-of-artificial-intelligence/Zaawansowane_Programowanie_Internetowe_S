using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Utilities;
using System.Text.RegularExpressions;

namespace Application.Domain;

public sealed record EmailAddress
{

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static Result<EmailAddress> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !EmailRegex.IsMatch(value))
        {
            return Result.Failure<EmailAddress>("Niepoprawny format adresu email.");
        }

        return Result.Success(new EmailAddress(value));
    }
}
