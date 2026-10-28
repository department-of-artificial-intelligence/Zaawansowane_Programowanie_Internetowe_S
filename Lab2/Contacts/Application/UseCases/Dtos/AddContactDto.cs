using Application.Domain;
using Application.Utilities;
using DomainAge = Application.Domain.Age;
using DomainFirstName = Application.Domain.FirstName;
using DomainLastName = Application.Domain.LastName;

namespace Application.UseCases.Dtos;

public class AddContactDto(string firstName, string lastName, Sex sex, int age)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public Sex Sex { get; } = sex;
    public int Age { get; } = age;

    public Result<Contact> ToDomain() =>
        DomainFirstName.Create(FirstName)
            .Bind(first => DomainLastName.Create(LastName)
            .Bind(last => DomainAge.Create(Age)
            .Bind(years => Contact.Create(first, last, Sex, years))));
}
