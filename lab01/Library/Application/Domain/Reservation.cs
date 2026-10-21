using Application.Utilities;

namespace Application.Domain;

public enum ReservationStatus { Active, Fulfilled, Cancelled }

public class Reservation : Entity
{
    public Guid BookId { get; private set; }
    public Guid ReaderId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ReservationStatus Status { get; private set; }

    public bool IsActive => Status == ReservationStatus.Active;

    private Reservation(Guid id, Guid bookId, Guid readerId, DateTime createdAt, ReservationStatus status)
        : base(id)
    {
        BookId = bookId;
        ReaderId = readerId;
        CreatedAt = createdAt;
        Status = status;
    }

    // Rezerwację tworzy wyłącznie LendingPolicy, która sprawdza reguły biznesowe.
    internal static Reservation Create(Guid bookId, Guid readerId, DateTime now) =>
        new(Guid.NewGuid(), bookId, readerId, now, ReservationStatus.Active);

    public Result<Unit> Cancel()
    {
        if (!IsActive)
        {
            return Result.Failure<Unit>("Rezerwacja nie jest aktywna.");
        }

        Status = ReservationStatus.Cancelled;
        return Result.Success(Unit.Instance);
    }

    internal void Fulfill() => Status = ReservationStatus.Fulfilled;
}
