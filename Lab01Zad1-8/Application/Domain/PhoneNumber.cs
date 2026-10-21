using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed record PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string value) =>
        Validators.NotEmpty(value, "Numer telefonu nie może być pusty")
            .Bind(_ => 
            {
                var regex = new Regex(@"^\+?[0-9\s-]{9,15}$");
                return regex.IsMatch(value)
                    ? Result.Success(new PhoneNumber(value))
                    : Result.Failure<PhoneNumber>("Niepoprawny numer telefonu");
            });
}