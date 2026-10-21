using Application.Utilities;

namespace Application.Domain;

public class Phone
{
    public PhoneNumber Number { get; private set; }
    public Guid CategoryId { get; private set; }

    private Phone() 
    {
        Number = null!;
    }

    private Phone(PhoneNumber number, Guid categoryId)
    {
        Number = number;
        CategoryId = categoryId;
    }

    public static Result<Phone> Create(PhoneNumber number, Guid categoryId) =>
        Result.Success(new Phone(number, categoryId));
}