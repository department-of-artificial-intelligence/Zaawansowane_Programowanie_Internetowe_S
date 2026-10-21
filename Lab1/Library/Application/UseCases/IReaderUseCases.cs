using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IReaderUseCases
{
    Result<AddReaderResult> AddReader(AddReaderDto addReaderDto);
    Result<Unit> UpdateReader(UpdateReaderDto updateReaderDto);
    Result<Unit> RemoveReader(Guid id);
}
