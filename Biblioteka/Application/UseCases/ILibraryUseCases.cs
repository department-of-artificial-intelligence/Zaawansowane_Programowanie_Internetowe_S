using Application.UseCases.Dtos;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public interface IBookUseCases
    {
        Result<AddBookResult> AddBook(AddBookDto dto);
    }

    public interface IReaderUseCases
    {
        Result<RegisterReaderResult> RegisterReader(RegisterReaderDto dto);
    }

    public interface ILoanUseCases
    {
        Result<BorrowResult> BorrowCopy(BorrowBookCopyDto dto);
        Result<Unit> ReturnCopy(ReturnBookCopyDto dto);
        Result<Unit> ExtendLoan(ExtendLoanDto dto);
    }
}
