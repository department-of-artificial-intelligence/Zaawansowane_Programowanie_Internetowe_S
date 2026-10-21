using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class ReaderServices(
    IReaderRepository readerRepository,
    IReservationRepository reservationRepository,
    LendingPolicy lendingPolicy,
    IUnitOfWork unitOfWork
) : IReaderUseCases
{
    public Result<ReaderResult> RegisterReader(RegisterReaderDto registerReaderDto)
        => registerReaderDto
            .ToDomain()
            .Tap(readerRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(ReaderResult.FromDomain);

    // Usunięcie czytelnika anuluje jego rezerwacje.
    public Result<Unit> RemoveReader(Guid readerId)
        => readerRepository
            .GetById(readerId)
            .Required($"Brak czytelnika (id: {readerId})")
            .Bind(reader => lendingPolicy.EnsureReaderCanBeRemoved(reader).Map(_ => reader))
            .Tap(reader =>
            {
                foreach (var reservation in reservationRepository.GetActiveByReader(reader.Id))
                    reservation.Cancel();
            })
            .Tap(readerRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
}
