using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IReaderRepository
{
    Result<Reader> GetById(Guid id);
    void Add(Reader reader);
}