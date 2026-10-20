using Application.Utilities;
namespace Application.Domain;

public sealed record LastName
{
    public const int MaxChars = 50;
    public string Value { get; }

    private LastName(string value) => Value = value;

    public static Result<LastName> Create(string value) =>
        Validators.NotEmpty(value, "Nazwisko jest wymagane.")
            .Bind(_ => Validators.MaxLength(value.Trim(), MaxChars,
                $"Nazwisko może mieć maksymalnie {MaxChars} znaków."))
            .Map(_ => new LastName(value.Trim()));

    public override string ToString() => Value;
}