using Library.Application.Utilities;

namespace Library.Application.Domain;

public class Reader : Entity
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public int ActiveLoansCount { get; private set; }

    private Reader() : base(Guid.Empty) { }

    private Reader(Guid id, string fullName, string email) : base(id)
    {
        FullName = fullName;
        Email = email;
        ActiveLoansCount = 0;
    }

    public static Result<Reader> Create(string fullName, string email) =>
        Result.Combine(
            Validators.NotEmpty(fullName, "Imię i nazwisko czytelnika jest wymagane."),
            Validators.NotEmpty(email, "Email jest wymagany.")
        ).Map(_ => new Reader(Guid.NewGuid(), fullName.Trim(), email.Trim()));

    public Result<Unit> CanBorrow(int maxAllowedLoans = 5)
    {
        if (ActiveLoansCount >= maxAllowedLoans)
            return Result.Failure<Unit>($"Czytelnik osiągnął maksymalny limit wypożyczeń ({maxAllowedLoans}).");

        return Result.Success(Unit.Instance);
    }

    public void IncrementLoan() => ActiveLoansCount++;
    public void DecrementLoan()
    {
        if (ActiveLoansCount > 0) ActiveLoansCount--;
    }
}