using Application.Utilities;

namespace Application.Domain;

public sealed record ImportantDate
{
    public string Name { get; }
    public DateOnly Date { get; }

    private ImportantDate(string name, DateOnly date)
    {
        Name = name;
        Date = date;
    }

    public static Result<ImportantDate> Create(string name, DateOnly date) =>
        Validators.NotEmpty(name, "Nazwa ważnej daty jest wymagana.")
            .Map(_ => new ImportantDate(name.Trim(), date));
}
