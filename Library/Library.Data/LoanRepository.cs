using Library.Application.Domain;
using Library.Application.Repositories;
using Library.Application.Utilities;

namespace Library.Data;

public class LoanRepository(LibraryDbContext context) : ILoanRepository
{
    public void Add(Loan loan) => context.Loans.Add(loan);
    public Option<Loan> GetActiveLoan(Guid bookId)
    {
        var loan = context.Loans.FirstOrDefault(l => l.BookId == bookId && l.ReturnDate == null);
        return loan is not null ? Option.Some(loan) : Option.None<Loan>();
    }
}