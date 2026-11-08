using Application.Utilities;

namespace Application.Domain;

public sealed record FirstName
{
    public string Name { get; }

    private FirstName(string n)
    {
        Name = n;
    }

    public static Result<FirstName> Create(string n) =>
        Validators
            .NotEmpty(n, "Imię nie może być puste.")
            .Map(_ => new FirstName(n));
}