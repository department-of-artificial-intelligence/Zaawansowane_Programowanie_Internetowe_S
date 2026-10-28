using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

internal static partial class UserNamePolicy
{
    public const int MinLength = 3;
    public const int MaxLength = 50;

    [GeneratedRegex(@"^[A-Za-z0-9._\-]+$")]
    private static partial Regex Pattern();

    public static Result<Unit> Validate(string userName) =>
        Validators
            .NotEmpty(userName, "Nazwa użytkownika nie może być pusta.")
            .Map(_ => userName.Trim())
            .Bind(trimmed => Result.Combine(
                Validators.InRange(trimmed.Length, MinLength, MaxLength,
                    $"Nazwa użytkownika musi mieć od {MinLength} do {MaxLength} znaków."),
                Validators.Matches(trimmed, Pattern(),
                    "Nazwa użytkownika może zawierać tylko litery, cyfry, kropki, podkreślenia i myślniki.")));
}

// Zadanie 3 - proste reguły walidacji hasła. Zwracane są wszystkie
// niespełnione reguły naraz, a nie tylko pierwsza.
internal static partial class PasswordPolicy
{
    public const int MinLength = 8;
    public const int MaxLength = 128;

    [GeneratedRegex(@"\p{Lu}")]
    private static partial Regex UpperCase();

    [GeneratedRegex(@"\p{Ll}")]
    private static partial Regex LowerCase();

    [GeneratedRegex(@"\d")]
    private static partial Regex Digit();

    public static Result<Unit> Validate(string password) =>
        Validators
            .NotEmpty(password, "Hasło nie może być puste.")
            .Bind(_ => Result.Combine(
                Validators.InRange(password.Length, MinLength, MaxLength,
                    $"Hasło musi mieć od {MinLength} do {MaxLength} znaków."),
                Validators.Matches(password, UpperCase(),
                    "Hasło musi zawierać co najmniej jedną wielką literę."),
                Validators.Matches(password, LowerCase(),
                    "Hasło musi zawierać co najmniej jedną małą literę."),
                Validators.Matches(password, Digit(),
                    "Hasło musi zawierać co najmniej jedną cyfrę.")));
}
