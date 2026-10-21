using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IReaderUseCases
{
    Result<ReaderResult> RegisterReader(RegisterReaderDto registerReaderDto);
    Result<Unit> RemoveReader(Guid readerId);
}
