using Application.Utilities;

namespace Application.Domain;

public sealed record UserProfile
{
    public static readonly UserProfile Empty = new("", "", default, "");

    public string FirstName { get; }
    public string LastName { get; }
    public DateOnly BirthDate { get; }
    public string Address { get; }

    private UserProfile(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string address)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Address = address;
    }

    public static Result<UserProfile> Create(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string address) =>
        Result.Combine(
                Validators.NotEmpty(firstName, "Imię nie może być puste."),
                Validators.NotEmpty(lastName, "Nazwisko nie może być puste."),
                Validators.NotEmpty(address, "Adres nie może być pusty."))
            .Map(_ => new UserProfile(firstName, lastName, birthDate, address));
}
