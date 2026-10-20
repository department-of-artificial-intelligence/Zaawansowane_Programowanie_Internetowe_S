using Application.Utilities;
namespace Application.Domain;

public sealed record FirstName
{
    public const int MaxChars = 50;
    public string Value { get; }

    private FirstName(string value) => Value = value;

    public static Result<FirstName> Create(string value) =>
        Validators.NotEmpty(value, "Imię jest wymagane.")
            .Bind(_ => Validators.MaxLength(value.Trim(), MaxChars,
                $"Imię może mieć maksymalnie {MaxChars} znaków."))
            .Map(_ => new FirstName(value.Trim()));

    public override string ToString() => Value;
}