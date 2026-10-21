using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface ILoanUseCases
{
    Result<BorrowBookResult> BorrowBook(BorrowBookDto borrowBookDto);
    Result<Unit> ReturnBook(Guid loanId);
}
