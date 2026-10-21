using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IReaderRepository
{
    Option<Reader> GetById(Guid readerId);
    void Add(Reader reader);
    void Remove(Reader reader);
}
