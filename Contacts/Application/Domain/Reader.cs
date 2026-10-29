using System;
using Application.Utilities;

namespace Application.Domain;

public class Reader : Entity
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    private Reader() : base(Guid.Empty) { }

    private Reader(string firstName, string lastName, string email) : base(Guid.NewGuid())
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public static Result<Reader> Create(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<Reader>("Imię i nazwisko są wymagane.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            return Result.Failure<Reader>("Niepoprawny adres e-mail.");

        return Result.Success(new Reader(firstName.Trim(), lastName.Trim(), email.Trim()));
    }
}