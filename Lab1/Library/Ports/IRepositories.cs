using Library.Domain;

namespace Library.Ports;

public interface IBookRepository
{
    IEnumerable<Book> GetAll();
    Book? Get(Guid id);
    void Add(Book book);
    void Remove(Book book);
}

public interface IReaderRepository
{
    Reader? Get(Guid id);
    void Add(Reader reader);
}

public interface ILoanRepository
{
    Loan? GetActiveForBook(Guid bookId);
    Loan? GetActiveForReader(Guid readerId);
    Loan? Get(Guid id);
    void Add(Loan loan);
}

public interface IReservationRepository
{
    bool HasActive(Guid bookId, Guid readerId);
    void Add(Reservation reservation);
    Reservation? Get(Guid id);
}

public interface IUnitOfWork
{
    void Save();
}
