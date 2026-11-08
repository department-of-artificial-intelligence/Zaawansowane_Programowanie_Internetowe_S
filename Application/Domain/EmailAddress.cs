using Application.Utilities;

namespace Application.Domain;

public sealed record EmailAddress
{
    public string Value {get;}

    private EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !Regex.IsMatch(value,  @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))          // [^@\s] - coś innego niż @ lub spacja
        {
            throw new ArgumentException(
                "Podano niepoprawny adres e-mail.");
        }

        Value = value;
    }

    public static Result<EmailAddress> Create(string value) =>
        Result.Success(new EmailAddress(value));
}