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
    Validators.NotEmpty(value, "Imię nie może być puste.")
    .Map(_ => new FirstName(value));
}