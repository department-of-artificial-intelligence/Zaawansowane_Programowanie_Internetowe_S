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
    Validators.NotEmpty(value, "Nazwisko nie może być puste.")
    .Map(_ => new LastName(value));
}