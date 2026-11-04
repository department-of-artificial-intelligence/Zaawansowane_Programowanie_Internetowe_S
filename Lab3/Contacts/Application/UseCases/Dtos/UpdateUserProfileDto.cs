using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public class UpdateUserProfileDto(
    Guid id, string firstName, string lastName, DateOnly birthDate, string address)
{
    public Guid Id { get; } = id;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public DateOnly BirthDate { get; } = birthDate;
    public string Address { get; } = address;

    public Result<Unit> ApplyTo(User user) =>
        UserProfile
            .Create(FirstName, LastName, BirthDate, Address)
            .Tap(user.SetProfile)
            .Map(_ => Unit.Instance);
}
