using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Application.Utilities;

namespace Application.Domain;

public class UserProfile
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateTime? BirthDate { get; private set; }
    public string Address { get; private set; } = string.Empty;

    // Konstruktor bezparametrowy wymagany przez EF Core
    private UserProfile() { }

    private UserProfile(string firstName, string lastName, DateTime? birthDate, string address)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Address = address;
    }

    public static Result<UserProfile> Create(string firstName, string lastName, DateTime? birthDate = null, string address = "")
    {
        return Validators.NotEmpty(firstName, "Imię nie może być puste")
            .Bind(_ => Validators.NotEmpty(lastName, "Nazwisko nie może być puste"))
            .Bind(_ => birthDate.HasValue && birthDate.Value > DateTime.UtcNow
                ? Result.Failure<Unit>("Data urodzenia nie może być z przyszłości")
                : Result.Success(Unit.Instance))
            .Map(_ => new UserProfile(firstName, lastName, birthDate, address ?? string.Empty));
    }
}
