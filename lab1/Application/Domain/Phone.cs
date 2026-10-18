
using Application.Utilities;

namespace Application.Domain;

public class Phone : Entity
{
    public PhoneNumber Number { get; private set; }

    private Phone(Guid id, PhoneNumber number) : base(id)
    {
        Number = number;
    }

    private Phone() : base(Guid.Empty)
    {
        Number = null!;
    }

    public static Result<Phone> Create(PhoneNumber number)
    {
        return Result.Success(
            new Phone(Guid.NewGuid(), number));
    }
}
