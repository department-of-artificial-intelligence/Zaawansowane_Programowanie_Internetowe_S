using Application.Domain;

namespace Application.UseCases.Dtos;

public class UpdateContactDto(
    Guid userId, Guid contactId, string firstName, string lastName, Sex sex, int age)
{
    public Guid UserId { get; } = userId;
    public Guid ContactId { get; } = contactId;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public Sex Sex { get; } = sex;
    public int Age { get; } = age;
}
