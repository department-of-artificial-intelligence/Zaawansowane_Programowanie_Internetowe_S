namespace Application.Domain;

// Parametry reguł biznesowych biblioteki.
public static class LibraryRules
{
    public const int MaxActiveLoans = 5;
    public const int MaxActiveReservations = 3;
    public const int LoanPeriodDays = 30;
    public const int ExtensionDays = 14;
}
