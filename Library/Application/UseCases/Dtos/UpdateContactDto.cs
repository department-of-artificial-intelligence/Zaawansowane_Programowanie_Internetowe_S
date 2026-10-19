using Application.Domain;
namespace Application.UseCases.Dtos;

public class UpdateContactDto(Guid id, string firstName, string lastName, Sex sex, int age)
{
    public Guid Id { get; } = id;
    public string FirstName { get;} = firstName;
    public string LastName { get;} = lastName;
    public Sex Sex { get; } = sex;
    public int Age { get; } = age;
}