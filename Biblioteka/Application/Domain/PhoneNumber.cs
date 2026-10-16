using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed record PhoneNumber
{
    // Obsługuje formaty np.: 123456789, +48123456789, 123-456-789
    private static readonly Regex PhoneRegex = new(
        @"^(\+?[0-9]{1,3})?[\s-]?[0-9]{3}[\s-]?[0-9]{3}[\s-]?[0-9]{3,4}$",
        RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<PhoneNumber>("Numer telefonu nie może być pusty.");
        }

        var normalized = value.Trim();
        if (!PhoneRegex.IsMatch(normalized))
        {
            return Result.Failure<PhoneNumber>($"Niepoprawny format numeru telefonu: '{value}'.");
        }

        return Result.Success(new PhoneNumber(normalized));
    }
}
