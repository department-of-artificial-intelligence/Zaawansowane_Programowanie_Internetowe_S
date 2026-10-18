
using Application.Utilities;

namespace Application.Domain;

public record EventName
{
    public string Value { get; }

    private EventName(string value)
    {
        Value = value;
    }

    public static Result<EventName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<EventName>(
                "Nazwa wydarzenia nie może być pusta.");

        value = value.Trim();

        if (value.Length > 100)
            return Result.Failure<EventName>(
                "Nazwa wydarzenia może mieć maksymalnie 100 znaków.");

        return Result.Success(new EventName(value));
    }
}
