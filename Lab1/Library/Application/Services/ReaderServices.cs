using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class ReaderServices(
    IReaderRepository readerRepository,
    IUnitOfWork unitOfWork,
    ReaderUniquenessChecker readerUniquenessChecker,
    LibraryRules libraryRules
) : IReaderUseCases
{
    public Result<AddReaderResult> AddReader(AddReaderDto addReaderDto)
        => addReaderDto
            .ToDomain()
            .Bind(reader =>
                readerUniquenessChecker
                    .EnsureUnique(reader.Email)
                    .Map(_ => reader)
            )
            .Tap(readerRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddReaderResult.FromDomain);

    public Result<Unit> UpdateReader(UpdateReaderDto updateReaderDto)
        => readerRepository.GetById(updateReaderDto.Id)
            .Required($"Brak czytelnika (id: {updateReaderDto.Id})")
            .Bind(reader => reader.Update(
                updateReaderDto.FirstName,
                updateReaderDto.LastName))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveReader(Guid id)
        => readerRepository.GetById(id)
            .Required($"Brak czytelnika (id: {id})")
            .Bind(reader =>
                libraryRules
                    .EnsureReaderCanBeRemoved(reader)
                    .Map(_ => reader)
            )
            .Tap(readerRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
}
