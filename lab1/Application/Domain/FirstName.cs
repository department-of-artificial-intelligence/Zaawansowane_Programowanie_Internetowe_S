using Application.Utilities;

namespace Application.Domain;

public record FirstName
{
    public string Value { get; }

    private FirstName(string value)
    {
        Value = value;
    }

    public static Result<FirstName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<FirstName>("Imię nie może być puste.");

        value = value.Trim();

        if (value.Length < 2 || value.Length > 50)
            return Result.Failure<FirstName>(
                "Imię musi mieć od 2 do 50 znaków.");

        return Result.Success(new FirstName(value));
    }
}
