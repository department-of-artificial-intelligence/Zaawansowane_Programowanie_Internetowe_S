using Application.Utilities;
namespace Application.Domain;

// Zadanie 3: proste reguły poprawności hasła
public static class PasswordPolicy
{
    public const int MinChars = 8;
    public const int MaxChars = 100;

    public static Result<Unit> Validate(string password) =>
        Validators.NotEmpty(password, "Hasło nie może być puste.")
            .Bind(_ => Validators.IsTrue(
                password.Length is >= MinChars and <= MaxChars,
                $"Hasło musi mieć od {MinChars} do {MaxChars} znaków."))
            .Bind(_ => Validators.IsTrue(
                password.Any(char.IsLetter),
                "Hasło musi zawierać co najmniej jedną literę."))
            .Bind(_ => Validators.IsTrue(
                password.Any(char.IsDigit),
                "Hasło musi zawierać co najmniej jedną cyfrę."));
}
