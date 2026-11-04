using Application.Domain;
using Application.Utilities;
using DomainAge = Application.Domain.Age;

namespace Application.UseCases.Dtos;

public class EditContactDto(
    Guid id, string firstName, string lastName, Sex sex, int age)
{
    public Guid Id { get; } = id;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public Sex Sex { get; } = sex;
    public int Age { get; } = age;

    public Result<(FirstName FirstName, LastName LastName, Age Age)> ToDomain() =>
        Application.Domain.FirstName.Create(FirstName)
            .Bind(first => Application.Domain.LastName.Create(LastName)
                .Bind(last => DomainAge.Create(Age)
                    .Map(ageValue => (first, last, ageValue))));
}
