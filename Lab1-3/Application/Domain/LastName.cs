using Application.Utilities;

namespace Application.Domain;

public sealed record LastName
{
    public string Value { get; }

    private LastName(string value)
    {
        Value = value;
    }

    public static Result<LastName> Create(string value) =>
        Result.Combine(
            Validators.NotEmpty(value, "Nazwisko jest wymagane."),
            Validators.MaxLength(value, 100, "Nazwisko może mieć maksymalnie 100 znaków.")
        ).Map(_ => new LastName(value.Trim()));
}