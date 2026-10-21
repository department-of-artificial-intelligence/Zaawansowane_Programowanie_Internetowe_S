using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IReservationRepository
{
    Option<Reservation> GetById(Guid reservationId);
    IEnumerable<Reservation> GetActiveByReader(Guid readerId);
    IEnumerable<Reservation> GetActiveByBook(Guid bookId);
    void Add(Reservation reservation);
}
