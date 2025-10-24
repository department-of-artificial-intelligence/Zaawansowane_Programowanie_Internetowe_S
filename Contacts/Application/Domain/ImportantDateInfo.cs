namespace Contacts.Application.Domain;

public record ImportantDateInfo
{
    public DateOnly Date { get; private set; }
    public string Description { get; private set; } = string.Empty;

    // For EF Core
    private ImportantDateInfo() { }

    public ImportantDateInfo(DateOnly date, string description)
    {
        if (date == default)
            throw new ArgumentException("Data nie może być wartością domyślną", nameof(date));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Opis nie może być pusty", nameof(description));

        Date = date;
        Description = description.Trim();
        if (Description.Length > 200)
            Description = Description.Substring(0, 200);
    }

    public override string ToString() => $"{Date:yyyy-MM-dd} - {Description}";
}