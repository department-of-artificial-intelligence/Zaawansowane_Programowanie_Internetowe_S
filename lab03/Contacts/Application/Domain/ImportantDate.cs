using Application.Utilities;

namespace Application.Domain;

// Ważna data związana z kontaktem, np. urodziny lub rocznica (zadanie 8).
public sealed record ImportantDate
{
    public const int DescriptionMaxLength = 100;

    public DateOnly Date { get; }
    public string Description { get; }

    private ImportantDate(DateOnly date, string description)
    {
        Date = date;
        Description = description;
    }

    public static Result<ImportantDate> Create(DateOnly date, string description) =>
        Validators
            .NotEmpty(description, "Opis daty jest wymagany.")
            .Map(_ => description.Trim())
            .Bind(trimmed => Validators
                .MaxLength(trimmed, DescriptionMaxLength,
                    $"Opis daty może mieć maksymalnie {DescriptionMaxLength} znaków.")
                .Map(_ => new ImportantDate(date, trimmed)));
}
