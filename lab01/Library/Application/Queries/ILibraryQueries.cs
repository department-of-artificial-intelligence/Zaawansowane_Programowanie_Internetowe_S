using Application.Queries.Dtos;

namespace Application.Queries;

public interface ILibraryQueries
{
    IEnumerable<BookDto> GetBooks();
    IEnumerable<LoanDto> GetActiveLoans(Guid readerId);
    IEnumerable<LoanDto> GetOverdueLoans();
}
