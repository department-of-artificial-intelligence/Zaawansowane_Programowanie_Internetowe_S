using Application.Domain;
using Application.OutputPorts;
using Application.Queries;
using Application.Queries.Dtos;

namespace Data;

public class LibraryQueries(AppDbContext context, IClock clock) : ILibraryQueries
{
    public IEnumerable<BookDto> GetBooks() =>
        context.Books
            .Select(book => new
            {
                Book = book,
                ActiveLoans = context.Loans.Count(l => l.BookId == book.Id && l.ReturnDate == null)
            })
            .ToList()
            .Select(x => new BookDto(
                x.Book.Id, x.Book.Isbn.Value, x.Book.Title, x.Book.Author,
                x.Book.Copies, x.Book.Copies - x.ActiveLoans));

    public IEnumerable<LoanDto> GetActiveLoans(Guid readerId) =>
        QueryActiveLoans(context.Loans.Where(l => l.ReaderId == readerId));

    public IEnumerable<LoanDto> GetOverdueLoans()
    {
        var today = clock.Today;
        return QueryActiveLoans(context.Loans.Where(l => l.DueDate < today));
    }

    private IEnumerable<LoanDto> QueryActiveLoans(IQueryable<Loan> loans) =>
        (from loan in loans
         where loan.ReturnDate == null
         join book in context.Books on loan.BookId equals book.Id
         join reader in context.Readers on loan.ReaderId equals reader.Id
         select new LoanDto(
             loan.Id, book.Title, reader.FirstName + " " + reader.LastName,
             loan.LoanDate, loan.DueDate))
        .ToList();
}
