using Application.Domain;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class ReaderServices(
    IReaderRepository readerRepository,
    IUnitOfWork unitOfWork
) : IReaderUseCases
{
    public Result<AddReaderResult> AddReader(AddReaderDto addReaderDto)
        => addReaderDto
            .ToDomain()
            .Tap(readerRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddReaderResult.FromDomain);

    public Result<Unit> RemoveReader(Guid id)
        => readerRepository.GetById(id)
            .Required($"Nie mogę odnaleźć czytelnika o id = {id}")
            .Tap(readerRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> UpdateReader(UpdateReaderDto updateReaderDto)
        => readerRepository.GetById(updateReaderDto.Id)
            .Required($"Nie mogę odnaleźć czytelnika o id = {updateReaderDto.Id}")
            .Bind(reader =>
                FullName.Create(updateReaderDto.FirstName, updateReaderDto.LastName)
                    .Bind(fullName => EmailAddress.Create(updateReaderDto.Email)
                        .Bind(email => reader.UpdateDetails(fullName, email))
                    )
            )
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

}