using Application.Domain;
namespace Application.UseCases.Dtos;
public class AddUserResult(Guid id, string name, string firstName, string lastName, DateTime? dateOfBirth, string address)
{
    public Guid Id { get; } = id;
    public string UserName { get; } = name;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public DateTime? DateOfBirth { get; } = dateOfBirth;
    public string Address { get; } = address;
    internal static AddUserResult FromDomain(User user) =>
        new (user.Id, user.UserName, user.FirstName.Value, user.LastName.Value, user.DateOfBirth, user.Address);
}