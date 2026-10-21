using Application.Utilities;

namespace Application.Domain;

public class Reservation : Entity
{
    public Guid BookId { get; private set; }
    public Guid ReaderId { get; private set; }

    private Reservation(Guid id, Guid bookId, Guid readerId) : base(id)
    {
        BookId = bookId;
        ReaderId = readerId;
    }

    public static Result<Reservation> Create(Guid bookId, Guid readerId) =>
        Result.Success(new Reservation(Guid.NewGuid(), bookId, readerId));
}
