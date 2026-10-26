using Application.Domain;
using Application.Utilities;
using DomainAge = Application.Domain.Age;
namespace Application.UseCases.Dtos;

public class AddContactDto(string firstName, string lastName, Sex sex, int age)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public Sex Sex { get; } = sex;
    public int Age { get; } = age;
    public Result<Contact> ToDomain() =>
    DomainAge.Create(Age)
    .Bind(age => Contact.Create(FirstName, LastName, Sex, age));
}