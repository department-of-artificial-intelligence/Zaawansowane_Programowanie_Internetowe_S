using Application.Domain;
using Application.Utilities;
using DomainAge = Application.Domain.Age;
using DomainFirstName = Application.Domain.FirstName;
using DomainLastName = Application.Domain.LastName;

namespace Application.UseCases.Dtos;

public class UpdateContactDto(Guid id, string firstName, string lastName, Sex sex, int age)
{
    public Guid Id { get; } = id;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public Sex Sex { get; } = sex;
    public int Age { get; } = age;

    public Result<(FirstName FirstName, LastName LastName, Sex Sex, DomainAge Age)> Validate() =>
        DomainFirstName.Create(FirstName)
            .Bind(fName => DomainLastName.Create(LastName)
                .Bind(lName => DomainAge.Create(Age)
                    .Map(age => (fName, lName, Sex, age))));
}