using Application.Utilities;

namespace Library.Domain;

public sealed class Reservation
{
    private Reservation(Guid id, Guid bookId, Guid readerId, DateOnly date)
    {
        Id = id;
        BookId = bookId;
        ReaderId = readerId;
        ReservedOn = date;
    }

    public Guid Id { get; }
    public Guid BookId { get; }
    public Guid ReaderId { get; }
    public DateOnly ReservedOn { get; }
    public bool IsActive { get; private set; } = true;

    public static Result<Reservation> Create(
        Guid bookId, Guid readerId, DateOnly date) =>
        Result.Success(new Reservation(Guid.NewGuid(), bookId, readerId, date));

    public Result<Unit> Cancel() =>
        !IsActive
            ? Result.Failure<Unit>("Rezerwacja została już anulowana.")
            : Result.Success(CancelInternal());

    private Unit CancelInternal()
    {
        IsActive = false;
        return Unit.Instance;
    }
}
