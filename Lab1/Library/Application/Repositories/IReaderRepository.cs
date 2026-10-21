using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IReaderRepository
{
    IEnumerable<Reader> GetAll();
    Option<Reader> GetById(Guid readerId);
    Option<Reader> GetByEmail(EmailAddress email);
    void Add(Reader reader);
    void Remove(Reader reader);
}
