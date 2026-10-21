using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface ILoanRepository
{
    Option<Loan> GetById(Guid loanId);
    IEnumerable<Loan> GetByReader(Guid readerId);
    IEnumerable<Loan> GetByBook(Guid bookId);
    void Add(Loan loan);
}
