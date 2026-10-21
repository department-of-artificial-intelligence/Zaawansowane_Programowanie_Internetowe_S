using Application.Domain;

namespace Application.Queries.Dtos;

public class BookDto
{
    public Guid Id { get; }
    public string Title { get; }
    public string Author { get; }
    public string Isbn { get; }
    public int TotalCopies { get; }
    public int AvailableCopies { get; }

    private BookDto(
        Guid id,
        string title,
        string author,
        string isbn,
        int totalCopies,
        int availableCopies)
    {
        Id = id;
        Title = title;
        Author = author;
        Isbn = isbn;
        TotalCopies = totalCopies;
        AvailableCopies = availableCopies;
    }

    public static BookDto FromDomain(Book book) =>
        new(book.Id, book.Title, book.Author, book.Isbn.Value,
            book.TotalCopies, book.AvailableCopies);
}
