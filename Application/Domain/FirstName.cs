using Application.Utilities;

namespace Application.Domain;

public sealed record FirstName
{
    public string Value { get; }

    private FirstName(string value)
    {
        Value = value;
    }

    public static Result<FirstName> Create(string value) =>
        Result.Combine(
            Validators.NotEmpty(value, "Imię jest wymagane."),
            Validators.MaxLength(value, 50, "Imię może mieć maksymalnie 50 znaków.")
        ).Map(_ => new FirstName(value.Trim()));
}