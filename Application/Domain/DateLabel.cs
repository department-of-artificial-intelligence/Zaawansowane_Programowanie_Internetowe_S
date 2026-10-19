using Application.Utilities;

namespace Application.Domain;

public sealed record DateLabel
{
    public string Value { get; }

    private DateLabel(string value)
    {
        Value = value;
    }

    public static Result<DateLabel> Create(string value) =>
        Result.Combine(
            Validators.NotEmpty(value, "Nazwa daty jest wymagana."),
            Validators.MaxLength(value, 100, "Nazwa daty może mieć maksymalnie 100 znaków.")
        ).Map(_ => new DateLabel(value.Trim()));
}