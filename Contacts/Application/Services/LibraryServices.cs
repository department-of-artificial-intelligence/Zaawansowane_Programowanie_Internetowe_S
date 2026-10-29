using System;
using Application.Domain;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class LibraryServices : ILibraryUseCases
{
    private readonly IBookRepository _bookRepository;
    private readonly IReaderRepository _readerRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LibraryServices(
        IBookRepository bookRepository,
        IReaderRepository readerRepository,
        ILoanRepository loanRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _readerRepository = readerRepository;
        _loanRepository = loanRepository;
        _unitOfWork = unitOfWork;
    }

    public Result<AddBookResult> AddBook(AddBookDto dto)
    {
        return Book.Create(dto.Title, dto.Author, dto.Isbn)
            .Tap(book => _bookRepository.Add(book))
            .Tap(_ => _unitOfWork.Save())
            .Map(book => new AddBookResult(book.Id, book.Title));
    }

    public Result<RegisterReaderResult> RegisterReader(RegisterReaderDto dto)
    {
        return Reader.Create(dto.FirstName, dto.LastName, dto.Email)
            .Tap(reader => _readerRepository.Add(reader))
            .Tap(_ => _unitOfWork.Save())
            .Map(reader => new RegisterReaderResult(reader.Id, $"{reader.FirstName} {reader.LastName}"));
    }

    public Result<Unit> BorrowBook(BorrowBookDto dto)
    {
        return _bookRepository.GetById(dto.BookId)
            .Required($"Nie odnaleziono książki o ID = {dto.BookId}")
            .Bind(book => _readerRepository.GetById(dto.ReaderId)
                .Required($"Nie odnaleziono czytelnika o ID = {dto.ReaderId}")
                .Bind(_ => book.MarkAsLent())
                .Bind(_ => Loan.Create(dto.BookId, dto.ReaderId))
                .Tap(loan => _loanRepository.Add(loan)))
            .Tap(_ => _unitOfWork.Save())
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> ReturnBook(ReturnBookDto dto)
    {
        return _loanRepository.GetById(dto.LoanId)
            .Required($"Nie odnaleziono wypożyczenia o ID = {dto.LoanId}")
            .Bind(loan => _bookRepository.GetById(loan.BookId)
                .Required($"Nie odnaleziono książki o ID = {loan.BookId}")
                .Bind(book => book.MarkAsAvailable())
                .Bind(_ => loan.CompleteReturn()))
            .Tap(_ => _unitOfWork.Save())
            .Map(_ => Unit.Instance);
    }
}