using Application.Domain;
using Application.Utilities;
using DomainAge = Application.Domain.Age;
using DomainFirstName = Application.Domain.FirstName;
using DomainLastName = Application.Domain.LastName;

namespace Application.UseCases.Dtos;

public class AddContactDto(string firstName, string lastName, Sex sex, int age){
	public string FirstName { get; } = firstName;
	public string LastName { get; } = lastName;
	public Sex Sex { get; } = sex;
	public int Age { get; } = age;

	public Result<Contact> ToDomain() => DomainFirstName
		.Create(FirstName)
		.Bind(firstName => DomainLastName.Create(LastName)
			.Map(lastName => (firstName, lastName)))
		.Bind(names => DomainAge.Create(Age)
			.Bind(age => Contact.Create(names.firstName, names.lastName, Sex, age)));
}
