using Library.Application.Utilities;
using Library.Application.Domain;

namespace Library.Application.Repositories;

public interface ILoanRepository
{
    Option<Loan> GetActiveLoan(Guid bookId);
    void Add(Loan loan);
}