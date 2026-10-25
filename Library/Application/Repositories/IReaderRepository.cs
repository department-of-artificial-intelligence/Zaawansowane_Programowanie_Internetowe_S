using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IReaderRepository
{
    IEnumerable<Reader> GetAll();
    Option<Reader> GetById(Guid readerId);
    void Add(Reader reader);
    void Update(Reader reader);
    void Remove(Reader reader);
}