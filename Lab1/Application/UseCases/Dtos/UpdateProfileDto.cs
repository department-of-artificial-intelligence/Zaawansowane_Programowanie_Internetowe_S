namespace Application.UseCases.Dtos;

public class UpdateProfileDto(string firstName, string lastName, DateTime? birthDate, string address)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public DateTime? BirthDate { get; } = birthDate;
    public string Address { get; } = address;
}
