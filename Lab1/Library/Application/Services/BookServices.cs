using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class BookServices(
    IBookRepository bookRepository,
    IUnitOfWork unitOfWork,
    BookUniquenessChecker bookUniquenessChecker,
    LibraryRules libraryRules
) : IBookUseCases
{
    public Result<AddBookResult> AddBook(AddBookDto addBookDto)
        => addBookDto
            .ToDomain()
            .Bind(book =>
                bookUniquenessChecker
                    .EnsureUnique(book.Isbn)
                    .Map(_ => book)
            )
            .Tap(bookRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddBookResult.FromDomain);

    public Result<Unit> UpdateBook(UpdateBookDto updateBookDto)
        => bookRepository.GetById(updateBookDto.Id)
            .Required($"Brak książki (id: {updateBookDto.Id})")
            .Bind(book => book.Update(
                updateBookDto.Title,
                updateBookDto.Author,
                updateBookDto.TotalCopies))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveBook(Guid id)
        => bookRepository.GetById(id)
            .Required($"Brak książki (id: {id})")
            .Bind(book =>
                libraryRules
                    .EnsureBookCanBeRemoved(book)
                    .Map(_ => book)
            )
            .Tap(bookRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
}
