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
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LastName>("Nazwisko nie może być puste.");

        value = value.Trim();

        if (value.Length < 2 || value.Length > 100)
            return Result.Failure<LastName>(
                "Nazwisko musi mieć od 2 do 100 znaków.");

        return Result.Success(new LastName(value));
    }
}
