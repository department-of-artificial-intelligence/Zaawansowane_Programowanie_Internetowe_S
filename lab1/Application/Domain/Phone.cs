
using Application.Utilities;

namespace Application.Domain;

public sealed record Phone
{
    public PhoneNumber Number { get; }

    private Phone(PhoneNumber number)
    {
        Number = number;
    }

    internal static Result<Phone> Create(PhoneNumber number) =>
        Result.Success(new Phone(number));
}
