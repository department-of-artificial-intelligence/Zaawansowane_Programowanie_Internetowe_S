using Application.Utilities;

namespace Application.Domain;

public sealed record LastName
{
    public string Name { get; }

    private LastName(string n)
    {
        Name = n;
    }

    public static Result<LastName> Create(string n) =>
        Validators
            .NotEmpty(n, "Nazwisko nie może być puste.")
            .Map(_ => new LastName(n));
}