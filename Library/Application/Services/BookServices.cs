using Application.Domain;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class BookServices(
    IBookRepository bookRepository,
    IUnitOfWork unitOfWork
) : IBookUseCases
{
    public Result<AddBookResult> AddBook(AddBookDto addBookDto)
        => addBookDto
            .ToDomain()
            .Tap(bookRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddBookResult.FromDomain);

    public Result<Unit> RemoveBook(Guid id)
        => bookRepository.GetById(id)
            .Required($"Nie mogę odnaleźć ksiązki o id = {id}")
            .Tap(bookRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> UpdateBook(UpdateBookDto updateBookDto)
        => bookRepository.GetById(updateBookDto.Id)
            .Required($"Nie mogę odnaleźć ksiązki o id = {updateBookDto.Id}")
            .Bind(book =>
                FullName.Create(updateBookDto.AuthorFirstName, updateBookDto.AuthorLastName)
                    .Bind(authorName => Author.Create(authorName))
                    .Bind(author => BookTitle.Create(updateBookDto.BookTitle)
                        .Bind(title => BookCopies.Create(updateBookDto.BookCopies)
                            .Bind(copies => book.UpdateDetails(author, title, copies))
                        )
                    )
            )
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> BorrowBook(BorrowBookDto borrowBookDto)
        => bookRepository.GetById(borrowBookDto.BookId)
            .Required($"Nie mogę odnaleźć ksiązki o id = {borrowBookDto.BookId}")
            .Bind(book => book.BorrowBook())
            .Tap(_=> unitOfWork.Save());

    public Result<Unit> ReturnBook(ReturnBookDto returnBookDto)
        => bookRepository.GetById(returnBookDto.BookId)
            .Required($"Nie mogę odnaleźć ksiązki o id = {returnBookDto.BookId}")
            .Bind(book => book.ReturnBook())
            .Tap(_=> unitOfWork.Save());
}