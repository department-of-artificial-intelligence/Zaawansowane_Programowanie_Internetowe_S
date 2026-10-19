using Application.Utilities;
using Library.Application.Domain;

namespace Library.Application.Repositories;

public interface IReaderRepository
{
    Option<Reader> GetById(Guid id);
    IEnumerable<Reader> GetAll();
    void Add(Reader reader);
}