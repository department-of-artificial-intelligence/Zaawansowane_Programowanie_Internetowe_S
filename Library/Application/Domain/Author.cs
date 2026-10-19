using Application.Utilities;
using Application.Domain;

namespace Application.Domain;

public sealed record Author
{
    public string FirstName {get;}
    public string LastName {get;}

    private Author(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<Author> Create(string firstName, string lastName) =>
        Validators
            .NotEmpty(firstName, "Imie nie może być puste.")
            .NotEmpty(lastName, "Nazwisko nie może być puste.")
            .Map(_ => new  Author(firstName, lastName));
}