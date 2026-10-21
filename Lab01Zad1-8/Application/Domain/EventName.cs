using Application.Utilities;

namespace Application.Domain;

public sealed record EventName
{
    public string Value { get; }

    private EventName(string value)
    {
        Value = value;
    }

    public static Result<EventName> Create(string value) =>
        Validators.NotEmpty(value, "Nazwa wydarzenia nie może być pusta")
            .Map(_ => new EventName(value));
}