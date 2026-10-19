using Application.Utilities;

namespace Application.Domain;

public class ImportantDate
{
    public EventName Name { get; private set; }
    public DateTime Date { get; private set; }

    private ImportantDate() 
    {
        Name = null!;
    }

    private ImportantDate(EventName name, DateTime date)
    {
        Name = name;
        Date = date;
    }

    public static Result<ImportantDate> Create(EventName name, DateTime date) =>
        Result.Success(new ImportantDate(name, date));
}