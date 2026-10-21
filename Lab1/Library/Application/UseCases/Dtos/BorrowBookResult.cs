using Application.Domain;

namespace Application.UseCases.Dtos;

public class BorrowBookResult(Guid loanId, DateOnly dueDate)
{
    public Guid LoanId { get; } = loanId;
    public DateOnly DueDate { get; } = dueDate;

    internal static BorrowBookResult FromDomain(Loan loan) =>
        new(loan.Id, loan.DueDate);
}
