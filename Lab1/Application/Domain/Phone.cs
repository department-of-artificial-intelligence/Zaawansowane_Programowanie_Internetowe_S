using Application.Utilities;

namespace Application.Domain;

public sealed record Phone{
    public PhoneNumber Number { get; }
    public Guid CategoryId { get; }

    private Phone(PhoneNumber number, Guid categoryId){
        Number = number;
        CategoryId = categoryId;
    }

    internal static Result<Phone> Create(PhoneNumber number, Guid categoryId) =>
        Result.Success(new Phone(number, categoryId));
}
