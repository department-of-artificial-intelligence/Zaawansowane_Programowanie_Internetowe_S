
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

    internal static Result<ImportantDate> Create(
        string name,
        DateOnly date)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<ImportantDate>(
                "Nazwa ważnej daty nie może być pusta.");
        }

        return Result.Success(
            new ImportantDate(name.Trim(), date));
    }
}
