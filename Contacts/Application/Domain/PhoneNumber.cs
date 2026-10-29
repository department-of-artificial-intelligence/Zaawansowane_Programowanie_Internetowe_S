using Application.Utilities;

namespace Application.Domain;

public record PhoneNumber
{
    public string Value { get; private set; } = null!;

    private PhoneNumber() { }

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


        var digitsOnly = value.Replace(" ", "").Replace("-", "").Replace("+", "");
        if (digitsOnly.Length < 9)
        {
            return Result.Failure<PhoneNumber>("Numer telefonu musi składać się z co najmniej 9 cyfr.");
        }

        return Result.Success(new PhoneNumber(value.Trim()));
    }

    public override string ToString() => Value;
}