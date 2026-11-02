using Application.Domain;
using Application.Utilities;
using DomainAge = Application.Domain.Age;

namespace Application.UseCases.Dtos;

public class AddContactDto(Guid userId, string firstName, string lastName, Sex sex, int age)
{
    public Guid UserId {get; } = userId;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public Sex Sex { get; } = sex;
    public int Age { get; } = age;

    public Result<Contact> ToDomain() =>
    Domain.FirstName.Create(FirstName)
        .Bind(firstName =>
            Domain.LastName.Create(LastName)
                .Bind(lastName =>
                    DomainAge.Create(Age)
                        .Bind(age => 
                            Contact.Create(UserId, firstName, lastName, Sex, age)
                        )
                )
        );
}