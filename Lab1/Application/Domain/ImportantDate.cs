namespace Application.Domain;

public class ImportantDate : Entity
{
    public DateLabel Label { get; private set; }
    public DateOnly Date { get; private set; }

    private ImportantDate(Guid id, DateLabel label, DateOnly date) : base(id)
    {
        Label = label;
        Date = date;
    }

    internal static ImportantDate Create(DateLabel label, DateOnly date) =>
        new(Guid.NewGuid(), label, date);

    internal void Change(DateLabel label, DateOnly date)
    {
        Label = label;
        Date = date;
    }
}