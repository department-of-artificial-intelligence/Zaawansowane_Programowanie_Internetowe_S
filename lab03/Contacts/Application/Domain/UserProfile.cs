using Application.Utilities;

namespace Application.Domain;

// Zadanie 4 - profil użytkownika. Wszystkie pola są opcjonalne,
// ale jeżeli zostaną podane, muszą być poprawne.
public sealed record UserProfile
{
    public const int AddressMaxLength = 200;

    public string? FirstName { get; }
    public string? LastName { get; }
    public DateOnly? DateOfBirth { get; }
    public string? Address { get; }

    private UserProfile(
        string? firstName,
        string? lastName,
        DateOnly? dateOfBirth,
        string? address)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Address = address;
    }

    public static UserProfile Empty { get; } = new(null, null, null, null);

    public static Result<UserProfile> Create(
        string? firstName,
        string? lastName,
        DateOnly? dateOfBirth,
        string? address,
        DateOnly today)
    {
        var first = Optional(firstName, value => Domain.FirstName.Create(value).Map(f => f.Value));
        var last = Optional(lastName, value => Domain.LastName.Create(value).Map(l => l.Value));
        var trimmedAddress = Optional(address, value =>
            Validators
                .MaxLength(value, AddressMaxLength,
                    $"Adres może mieć maksymalnie {AddressMaxLength} znaków.")
                .Map(_ => value));

        return Result.Combine(
                first.Map(_ => Unit.Instance),
                last.Map(_ => Unit.Instance),
                trimmedAddress.Map(_ => Unit.Instance),
                dateOfBirth is null || dateOfBirth <= today
                    ? Result.Success(Unit.Instance)
                    : Result.Failure<Unit>("Data urodzenia nie może być z przyszłości."))
            .Bind(_ => first.Bind(f => last.Bind(l => trimmedAddress.Map(a =>
                new UserProfile(f, l, dateOfBirth, a)))));
    }

    // Puste pole oznacza brak wartości, niepuste musi przejść walidację.
    private static Result<string?> Optional(
        string? value, Func<string, Result<string>> validate) =>
        string.IsNullOrWhiteSpace(value)
            ? Result.Success<string?>(null)
            : validate(value.Trim()).Map(v => (string?)v);
}
