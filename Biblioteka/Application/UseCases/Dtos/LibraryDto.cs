using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos
{
    public record AddBookDto(string Title, string Author, string Isbn, string[] InitialCopies);
    public record AddBookResult(Guid BookId, string Title, int CopiesCount);

    public record RegisterReaderDto(string FirstName, string LastName, string Email, string CardNumber);
    public record RegisterReaderResult(Guid ReaderId, string FullName, string CardNumber);

    public record BorrowBookCopyDto(Guid ReaderId, string CopyCode, DateOnly BorrowDate);
    public record BorrowResult(Guid LoanId, string CopyCode, DateOnly DueDate);

    public record ReturnBookCopyDto(string CopyCode, DateOnly ReturnDate);
    public record ExtendLoanDto(Guid LoanId, int AdditionalDays = 14);
}
