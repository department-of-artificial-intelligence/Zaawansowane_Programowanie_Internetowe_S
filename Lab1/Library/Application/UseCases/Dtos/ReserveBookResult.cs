using Application.Domain;

namespace Application.UseCases.Dtos;

public class ReserveBookResult(Guid reservationId)
{
    public Guid ReservationId { get; } = reservationId;

    internal static ReserveBookResult FromDomain(Reservation reservation) =>
        new(reservation.Id);
}
