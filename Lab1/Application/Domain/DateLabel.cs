using Application.Utilities;
namespace Application.Domain;

public sealed record DateLabel
{
    public const int MaxChars = 50;
    public string Value { get; }

    private DateLabel(string value) => Value = value;

    public static Result<DateLabel> Create(string value) =>
        Validators.NotEmpty(value, "Nazwa daty jest wymagana.")
            .Bind(_ => Validators.MaxLength(value.Trim(), MaxChars,
                $"Nazwa daty może mieć maksymalnie {MaxChars} znaków."))
            .Map(_ => new DateLabel(value.Trim()));

    public override string ToString() => Value;
}