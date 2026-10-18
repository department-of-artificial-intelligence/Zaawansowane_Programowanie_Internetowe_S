using Application.Utilities;

namespace Application.Domain;

public sealed record Age
{
    public int Value { get; }

    private Age(int value)
    {
        Value = value;
    }

    public static Result<Age> Create(int value) =>
        Validators
            .InRange(value, 18, 120, "Wiek ma niepoprawną wartość.")
            .Map(_ => new Age(value));
}