using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public class AddBookDto(string isbn, string title, string author, int copies)
{
    public string Isbn { get; } = isbn;
    public string Title { get; } = title;
    public string Author { get; } = author;
    public int Copies { get; } = copies;

    public Result<Book> ToDomain() =>
        Domain.Isbn
            .Create(Isbn)
            .Bind(isbn => Book.Create(isbn, Title, Author, Copies));
}

public class BookResult(Guid id, string isbn, string title, string author, int copies)
{
    public Guid Id { get; } = id;
    public string Isbn { get; } = isbn;
    public string Title { get; } = title;
    public string Author { get; } = author;
    public int Copies { get; } = copies;

    internal static BookResult FromDomain(Book book) =>
        new(book.Id, book.Isbn.Value, book.Title, book.Author, book.Copies);
}
