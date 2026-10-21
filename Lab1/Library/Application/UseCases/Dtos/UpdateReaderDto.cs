namespace Application.UseCases.Dtos;

public class UpdateReaderDto(Guid id, string firstName, string lastName)
{
    public Guid Id { get; } = id;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
}
