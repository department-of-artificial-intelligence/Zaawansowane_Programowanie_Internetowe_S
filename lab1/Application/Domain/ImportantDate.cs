
using Application.Utilities;

namespace Application.Domain;

public class ImportantDate : Entity
{
    public EventName Name { get; private set; }
    public DateOnly Date { get; private set; }

    private ImportantDate(
        Guid id,
        EventName name,
        DateOnly date) : base(id)
    {
        Name = name;
        Date = date;
    }

    private ImportantDate() : base(Guid.Empty)
    {
        Name = null!;
    }

    public static Result<ImportantDate> Create(
        EventName name,
        DateOnly date)
    {
        return Result.Success(
            new ImportantDate(Guid.NewGuid(), name, date));
    }
}
