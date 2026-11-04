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

    public Result<Unit> ApplyTo(Contact contact) =>
        DomainFirstName.Create(FirstName)
            .Bind(first => DomainLastName.Create(LastName)
            .Bind(last => DomainAge.Create(Age)
            .Bind(years => contact.Update(first, last, Sex, years))));
}
