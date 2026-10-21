using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IReservationRepository
{
    Option<Reservation> GetById(Guid reservationId);
    IEnumerable<Reservation> GetByReader(Guid readerId);
    IEnumerable<Reservation> GetByBook(Guid bookId);
    void Add(Reservation reservation);
    void Remove(Reservation reservation);
}
