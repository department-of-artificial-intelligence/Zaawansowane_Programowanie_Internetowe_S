using Application.Utilities;

namespace Application.Domain;

public sealed record ImportantDate
{
    public DateOnly Date {get;}
    public string Description {get;}

    private ImportantDate(DateOnly date, string description)
    {
        Date = date;
        Description = description;
    }

    public static Result<ImportantDate> Create(DateOnly date, string description) =>
        Validators
            .NotEmpty(description, "Opis nie może być pusty.")
            .Map(_ => new ImportantDate(date, description));
}