using Library.Domain;
using Library.Ports;

namespace Library.Infrastructure;

public sealed class InMemoryBookRepository : IBookRepository
{
    private readonly List<Book> items = [];
    public IEnumerable<Book> GetAll() => items;
    public Book? Get(Guid id) => items.SingleOrDefault(item => item.Id == id);
    public void Add(Book book) => items.Add(book);
    public void Remove(Book book) => items.Remove(book);
}

public sealed class InMemoryReaderRepository : IReaderRepository
{
    private readonly List<Reader> items = [];
    public Reader? Get(Guid id) => items.SingleOrDefault(item => item.Id == id);
    public void Add(Reader reader) => items.Add(reader);
}

public sealed class InMemoryLoanRepository : ILoanRepository
{
    private readonly List<Loan> items = [];
    public Loan? GetActiveForBook(Guid bookId) =>
        items.SingleOrDefault(item => item.BookId == bookId && item.IsActive);
    public Loan? GetActiveForReader(Guid readerId) =>
        items.SingleOrDefault(item => item.ReaderId == readerId && item.IsActive);
    public Loan? Get(Guid id) => items.SingleOrDefault(item => item.Id == id);
    public void Add(Loan loan) => items.Add(loan);
}

public sealed class InMemoryReservationRepository : IReservationRepository
{
    private readonly List<Reservation> items = [];
    public bool HasActive(Guid bookId, Guid readerId) =>
        items.Any(item => item.BookId == bookId &&
            item.ReaderId == readerId && item.IsActive);
    public void Add(Reservation reservation) => items.Add(reservation);
    public Reservation? Get(Guid id) =>
        items.SingleOrDefault(item => item.Id == id);
}

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public void Save()
    {
    }
}
