using Application.Utilities;

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

    // Konstruktor dla Entity Framework
    private Phone()
    {
        Number = null!;
    }

    internal static Result<Phone> Create(PhoneNumber number, PhoneType type) =>
        Validators.IsTrue(Enum.IsDefined(type), "Nieznany typ numeru telefonu.")
            .Map(_ => new Phone(number, type));
}