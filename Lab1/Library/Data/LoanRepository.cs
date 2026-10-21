using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Data;

public class LoanRepository(AppDbContext context) : ILoanRepository
{
    public void Add(Loan loan)
    {
        context.Loans.Add(loan);
    }

    public Option<Loan> GetById(Guid loanId)
    {
        var loan = context.Loans.Find(loanId);
        return loan is not null
            ? Option.Some(loan)
            : Option.None<Loan>();
    }

    public IEnumerable<Loan> GetByReader(Guid readerId) =>
        context.Loans.Where(loan => loan.ReaderId == readerId).ToList();

    public IEnumerable<Loan> GetByBook(Guid bookId) =>
        context.Loans.Where(loan => loan.BookId == bookId).ToList();
}
