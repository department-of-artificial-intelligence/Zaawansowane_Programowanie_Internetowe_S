using Application.Utilities;

namespace Application.Domain;

public sealed record PhoneNumber
{
    public string Value { get; }

    private PhoneNumber (string value)
    {
        Value = value;
    }
    private PhoneNumber()
    {
        Value = null!;
    }
    public static Result<PhoneNumber> Create(string value) =>
        Validators
            .NotEmpty(value, "Numer telefonu nie może być pusty.")
            .Map(_ => new PhoneNumber(value));
}