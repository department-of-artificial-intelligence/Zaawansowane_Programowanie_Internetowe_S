using Application.Domain;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LoanServices(
     ILoanRepository loanRepository,
     IBookRepository bookRepository,
     IReaderRepository readerRepository,
     IUnitOfWork unitOfWork
 ) : ILoanUseCases
    {
        public Result<BorrowResult> BorrowCopy(BorrowBookCopyDto dto) =>
            InventoryCode.Create(dto.CopyCode)
                .Bind(code =>
                    readerRepository.GetById(dto.ReaderId)
                        .Required($"Nie znaleziono czytelnika o id {dto.ReaderId}")
                        .Bind(reader =>
                        {
                            var activeCount = loanRepository.CountActiveLoansForReader(reader.Id);
                            return reader.EnsureCanBorrow(activeCount).Map(_ => reader);
                        })
                        .Bind(reader =>
                            bookRepository.GetByCopyCode(code)
                                .Required($"Egzemplarz o kodzie '{code.Value}' nie istnieje w żadnej książce.")
                                .Bind(book =>
                                    book.FindCopy(code)
                                        .Required($"Nie znaleziono egzemplarza {code.Value}")
                                        .Bind(copy => copy.MarkAsBorrowed())
                                        .Bind(_ => Loan.Create(reader.Id, book.Id, code, dto.BorrowDate))
                                )))
                .Tap(loanRepository.Add)
                .Tap(_ => unitOfWork.Save())
                .Map(loan => new BorrowResult(loan.Id, loan.CopyCode.Value, loan.DueDate));

        public Result<Unit> ReturnCopy(ReturnBookCopyDto dto) =>
            InventoryCode.Create(dto.CopyCode)
                .Bind(code =>
                    loanRepository.GetActiveLoanByCopyCode(code)
                        .Required($"Brak aktywnego wypożyczenia dla egzemplarza o kodzie '{code.Value}'.")
                        .Bind(loan =>
                            bookRepository.GetByCopyCode(code)
                                .Required("Nie odnaleziono powiązanej książki w katalogu.")
                                .Bind(book =>
                                    book.FindCopy(code)
                                        .Required("Nie odnaleziono egzemplarza.")
                                        .Bind(copy => copy.MarkAsReturned())
                                        .Bind(_ => loan.CompleteReturn(dto.ReturnDate)))))
                .Tap(_ => unitOfWork.Save());

        public Result<Unit> ExtendLoan(ExtendLoanDto dto) =>
            loanRepository.GetById(dto.LoanId)
                .Required($"Nie odnaleziono wypożyczenia o id {dto.LoanId}.")
                .Bind(loan => loan.Extend(dto.AdditionalDays))
                .Tap(_ => unitOfWork.Save());
    }
}
