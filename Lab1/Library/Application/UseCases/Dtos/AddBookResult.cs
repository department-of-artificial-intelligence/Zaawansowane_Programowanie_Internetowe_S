using Application.Domain;

namespace Application.UseCases.Dtos;

public class AddBookResult(
    Guid id, string title, string author, string isbn, int totalCopies)
{
    public Guid Id { get; } = id;
    public string Title { get; } = title;
    public string Author { get; } = author;
    public string Isbn { get; } = isbn;
    public int TotalCopies { get; } = totalCopies;

    internal static AddBookResult FromDomain(Book book) =>
        new(book.Id, book.Title, book.Author, book.Isbn.Value, book.TotalCopies);
}
