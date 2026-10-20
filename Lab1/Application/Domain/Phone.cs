namespace Application.Domain;

public sealed record Phone
{
    public PhoneNumber Number { get; }
    public PhoneType Type { get; }

    private Phone(PhoneNumber number, PhoneType type)
    {
        Number = number;
        Type = type;
    }

    internal static Phone Create(PhoneNumber number, PhoneType type) => new(number, type);
}