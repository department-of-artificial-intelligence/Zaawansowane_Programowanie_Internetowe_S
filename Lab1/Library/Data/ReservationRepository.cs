using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Data;

public class ReservationRepository(AppDbContext context) : IReservationRepository
{
    public void Add(Reservation reservation)
    {
        context.Reservations.Add(reservation);
    }

    public Option<Reservation> GetById(Guid reservationId)
    {
        var reservation = context.Reservations.Find(reservationId);
        return reservation is not null
            ? Option.Some(reservation)
            : Option.None<Reservation>();
    }

    public IEnumerable<Reservation> GetByReader(Guid readerId) =>
        context.Reservations.Where(r => r.ReaderId == readerId).ToList();

    public IEnumerable<Reservation> GetByBook(Guid bookId) =>
        context.Reservations.Where(r => r.BookId == bookId).ToList();

    public void Remove(Reservation reservation)
    {
        context.Remove(reservation);
    }
}
