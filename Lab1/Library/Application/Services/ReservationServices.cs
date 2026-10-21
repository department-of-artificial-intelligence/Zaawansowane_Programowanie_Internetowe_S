using Application.Domain;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class ReservationServices(
    IReservationRepository reservationRepository,
    IBookRepository bookRepository,
    IReaderRepository readerRepository,
    IUnitOfWork unitOfWork,
    LibraryRules libraryRules
) : IReservationUseCases
{
    public Result<ReserveBookResult> ReserveBook(ReserveBookDto reserveBookDto)
        => readerRepository.GetById(reserveBookDto.ReaderId)
            .Required($"Brak czytelnika (id: {reserveBookDto.ReaderId})")
            .Bind(reader =>
                bookRepository
                    .GetById(reserveBookDto.BookId)
                    .Required($"Brak książki (id: {reserveBookDto.BookId})")
                    .Bind(book =>
                        libraryRules
                            .EnsureCanReserve(reader, book)
                            .Bind(_ => Reservation.Create(book.Id, reader.Id))))
            .Tap(reservationRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(ReserveBookResult.FromDomain);

    public Result<Unit> CancelReservation(Guid reservationId)
        => reservationRepository.GetById(reservationId)
            .Required($"Brak rezerwacji (id: {reservationId})")
            .Tap(reservationRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
}
