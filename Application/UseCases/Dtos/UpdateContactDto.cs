using Application.Domain;
using Application.Utilities;
using DomainAge = Application.Domain.Age;
using DomainFirstName = Application.Domain.FirstName;
using DomainLastName = Application.Domain.LastName;

namespace Application.UseCases.Dtos;

public class UpdateContactDto(
    Guid id, string firstName, string lastName, Sex sex, int age)
{
    public Guid Id { get; } = id;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public Sex Sex { get; } = sex;
    public int Age { get; } = age;

    internal Result<(DomainFirstName firstName, DomainLastName lastName, DomainAge age)>
        ToValueObjects() =>
        DomainFirstName.Create(FirstName)
            .Bind(firstName => DomainLastName.Create(LastName)
            .Bind(lastName => DomainAge.Create(Age)
            .Map(age => (firstName, lastName, age))));
}