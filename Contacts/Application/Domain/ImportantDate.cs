using System;
using Application.Utilities;

namespace Application.Domain;

public record ImportantDate
{
    public string Name { get; private set; } = null!;
    public DateTime Date { get; private set; }

    private ImportantDate() { }

    private ImportantDate(string name, DateTime date)
    {
        Name = name;
        Date = date;
    }

    public static Result<ImportantDate> Create(string name, DateTime date)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<ImportantDate>("Nazwa wydarzenia nie może być pusta.");
        }

        return Result.Success(new ImportantDate(name.Trim(), date.Date));
    }
}