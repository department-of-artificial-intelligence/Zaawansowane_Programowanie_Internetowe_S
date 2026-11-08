using Application.Utilities;

namespace Application.Domain;

public sealed record PhoneNumber
{
    public string Value {get;}

    private PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !Regex.IsMatch(value,  @"^\d{3}-\d{3}-\d{3}$"))
        {
            throw new ArgumentException(
                "Podano niepoprawny numer telefonu.");
        }

        Value = value;
    }

    public static Result<PhoneNumber> Create(string value) =>
        Result.Success(new PhoneNumber(value));
}