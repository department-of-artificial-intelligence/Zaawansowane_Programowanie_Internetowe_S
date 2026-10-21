using Application.Domain;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class BookServices(
    IBookRepository bookRepository,
    IReservationRepository reservationRepository,
    LendingPolicy lendingPolicy,
    IUnitOfWork unitOfWork
) : IBookUseCases
{
    public Result<BookResult> AddBook(AddBookDto addBookDto)
        => addBookDto
            .ToDomain()
            .Bind(book => bookRepository.GetByIsbn(book.Isbn).Match(
                _ => Result.Failure<Book>($"Książka o ISBN {book.Isbn.Value} już istnieje w katalogu."),
                () => Result.Success(book)))
            .Tap(bookRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(BookResult.FromDomain);

    // Usunięcie książki anuluje oczekujące na nią rezerwacje.
    public Result<Unit> RemoveBook(Guid bookId)
        => bookRepository
            .GetById(bookId)
            .Required($"Brak książki (id: {bookId})")
            .Bind(book => lendingPolicy.EnsureBookCanBeRemoved(book).Map(_ => book))
            .Tap(book =>
            {
                foreach (var reservation in reservationRepository.GetActiveByBook(book.Id))
                    reservation.Cancel();
            })
            .Tap(bookRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
}
