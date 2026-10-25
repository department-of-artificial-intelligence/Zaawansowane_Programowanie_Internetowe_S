using Application.Utilities;

namespace Application.Domain;

public sealed record FullName
{
    public string FirstName {get;}
    public string LastName {get;}

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
    private FullName()
    {
        FirstName = null!;
        LastName = null!;
    }
    public static Result<FullName> Create(string firstName, string lastName) =>
        Result.Combine(
            Validators.NotEmpty(firstName, "Imię nie może być puste."),
            Validators.NotEmpty(lastName, "Nazwisko nie może być puste.")
        )
        .Map(_ => new  FullName(firstName, lastName));
}