using Application.Utilities;

namespace Application.Domain;

public class Reader : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    private Reader(Guid id, string firstName, string lastName) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<Reader> Create(string firstName, string lastName) =>
        Result.Combine(
                Validators.NotEmpty(firstName, "Imię jest wymagane."),
                Validators.NotEmpty(lastName, "Nazwisko jest wymagane."))
            .Map(_ => new Reader(Guid.NewGuid(), firstName.Trim(), lastName.Trim()));
}
