using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface ILoanRepository
{
    Option<Loan> GetById(Guid loanId);
    IEnumerable<Loan> GetActiveByReader(Guid readerId);
    IEnumerable<Loan> GetActiveByBook(Guid bookId);
    void Add(Loan loan);
}
