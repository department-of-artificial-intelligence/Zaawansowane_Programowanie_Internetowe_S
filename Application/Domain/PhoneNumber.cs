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
    private static readonly Regex PhoneRegex = new(
        @"^\+?[1-9]\d{1,14}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(250)
    );

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<PhoneNumber>("Numer telefonu jest wymagany.");
        }

        //Przykładowa walidacja formatu
        string cleaned = value.Trim().Replace(" ", "").Replace("-", "");
        if (!PhoneRegex.IsMatch(cleaned))
        {
            return Result.Failure<PhoneNumber>("Niepoprawny format numeru telefonu.");
        }

        return Result.Success(new PhoneNumber(cleaned));
    }
}
