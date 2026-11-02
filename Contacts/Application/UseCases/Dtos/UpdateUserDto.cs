namespace Application.UseCases.Dtos;

public class UpdateUserDto(
    string firstName, 
    string lastName, 
    DateTime? dateOfBirth = null, 
    string address = "")
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public DateTime? DateOfBirth { get; } = dateOfBirth;
    public string Address { get; } = address;
}