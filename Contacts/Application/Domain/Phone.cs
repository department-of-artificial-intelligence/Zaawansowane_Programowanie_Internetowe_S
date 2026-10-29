using System;
using Application.Utilities;

namespace Application.Domain;

public class Phone : Entity
{
    public PhoneNumber Number { get; private set; }
    public Guid CategoryId { get; private set; }

    private Phone() : base(Guid.Empty)
    {
        Number = null!;
    }

    private Phone(PhoneNumber number, Guid categoryId) : base(Guid.NewGuid())
    {
        Number = number;
        CategoryId = categoryId;
    }

    public static Result<Phone> Create(string number, Guid categoryId)
    {
        return PhoneNumber.Create(number)
            .Map(validNumber => new Phone(validNumber, categoryId));
    }
}