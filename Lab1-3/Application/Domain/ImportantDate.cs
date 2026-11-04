using Application.Utilities;

namespace Application.Domain;

public sealed record ImportantDate
{
    private static readonly DateOnly MinDate = new(1900, 1, 1);

    public DateOnly Date { get; }
    public DateLabel Label { get; }

    private ImportantDate(DateOnly date, DateLabel label)
    {
        Date = date;
        Label = label;
    }

    // Konstruktor dla Entity Framework
    private ImportantDate()
    {
        Label = null!;
    }

    internal static Result<ImportantDate> Create(DateOnly date, DateLabel label) =>
        Validators.IsTrue(date >= MinDate,
                "Data nie może być wcześniejsza niż 01.01.1900.")
            .Map(_ => new ImportantDate(date, label));
}