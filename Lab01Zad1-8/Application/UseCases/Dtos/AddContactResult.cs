using Application.Domain;

namespace Application.UseCases.Dtos;

public class AddContactResult(
    Guid id, FirstName firstName, LastName lastName, Sex sex, int age
)
{
    public Guid Id { get; } = id;
    public FirstName FirstName { get; } = firstName;
    public LastName LastName { get; } = lastName;
    public Sex Sex { get; } = sex;
    public int Age { get; } = age;

internal static AddContactResult FromDomain(Contact contact) =>
    new(contact.Id, contact.FirstName, contact.LastName,
        contact.Sex, contact.Age.Value);
}