using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IReservationUseCases
{
    Result<ReserveBookResult> ReserveBook(ReserveBookDto reserveBookDto);
    Result<Unit> CancelReservation(Guid reservationId);
}
