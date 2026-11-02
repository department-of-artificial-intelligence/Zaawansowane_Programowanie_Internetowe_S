using Application.Utilities;
namespace Application.Domain;

// Zadanie 4: profil użytkownika jako obiekt wartości (wszystkie pola opcjonalne)
public sealed record UserProfile
{
    public const int MaxNameChars = 50;
    public const int MaxAddressChars = 200;

    public string? FirstName { get; }
    public string? LastName { get; }
    public DateOnly? BirthDate { get; }
    public string? Address { get; }

    private UserProfile(string? firstName, string? lastName, DateOnly? birthDate, string? address)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Address = address;
    }

    public static UserProfile Empty => new(null, null, null, null);

    public static Result<UserProfile> Create(
        string? firstName, string? lastName, DateOnly? birthDate, string? address, DateOnly today) =>
        Validators.MaxLength(firstName?.Trim(), MaxNameChars,
                $"Imię może mieć maksymalnie {MaxNameChars} znaków.")
            .Bind(_ => Validators.MaxLength(lastName?.Trim(), MaxNameChars,
                $"Nazwisko może mieć maksymalnie {MaxNameChars} znaków."))
            .Bind(_ => Validators.MaxLength(address?.Trim(), MaxAddressChars,
                $"Adres może mieć maksymalnie {MaxAddressChars} znaków."))
            .Bind(_ => Validators.IsTrue(
                birthDate is null || (birthDate <= today && birthDate >= today.AddYears(-130)),
                "Niepoprawna data urodzenia."))
            .Map(_ => new UserProfile(
                Normalize(firstName), Normalize(lastName), birthDate, Normalize(address)));

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
