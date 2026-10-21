using Application.Utilities;

namespace Application.Domain;

public class Reader : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public EmailAddress Email { get; private set; }

    private Reader(
        Guid id,
        string firstName,
        string lastName,
        EmailAddress email) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public static Result<Reader> Create(
        string firstName,
        string lastName,
        EmailAddress email) =>
        Result.Combine(
            Validators.NotEmpty(firstName, "Imię jest wymagane."),
            Validators.NotEmpty(lastName, "Nazwisko jest wymagane.")
        ).Map(_ => new Reader(Guid.NewGuid(), firstName, lastName, email));

    public Result<Unit> Update(string firstName, string lastName) =>
        Result.Combine(
            Validators.NotEmpty(firstName, "Imię jest wymagane."),
            Validators.NotEmpty(lastName, "Nazwisko jest wymagane.")
        ).Tap(_ =>
        {
            FirstName = firstName;
            LastName = lastName;
        });
}
