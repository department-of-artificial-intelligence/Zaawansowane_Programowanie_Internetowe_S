using Application.Utilities;

namespace Application.Domain;

public sealed record ImportantDate{
    public string Description { get; }
    public DateOnly Date { get; }

    private ImportantDate(string description, DateOnly date){
        Description = description;
        Date = date;
    }

    public static Result<ImportantDate> Create(string description, DateOnly date) => Validators
    	.NotEmpty(description, "Opis ważnej daty nie może być pusty.")
    	.Map(_ => new ImportantDate(description, date));
}
