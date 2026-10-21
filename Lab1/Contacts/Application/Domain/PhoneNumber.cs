using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed record PhoneNumber
{
    private static readonly Regex Pattern = new(@"^\+?[0-9]{9,15}$");

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string value) =>
        Pattern.IsMatch(value)
            ? Result.Success(new PhoneNumber(value))
            : Result.Failure<PhoneNumber>("Numer telefonu ma niepoprawny format.");
}
