using Application.Utilities;
namespace Application.Domain;

public record LastName
{
    public string Value { get; }

    private LastName(string value)
    {
        Value = value;
    }

    public static Result<LastName> Create(string value)
    {
        return Validators.NotEmpty(value, "Nazwisko jest wymagane.")
            .Map(_ => new LastName(value));
    }

    public override string ToString() => Value;
}