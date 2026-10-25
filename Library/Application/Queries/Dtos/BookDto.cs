using Application.Domain;

namespace Application.Queries.Dtos;

public class BookDto
{
    public Guid Id { get; }
    public string AuthorFirstName { get; }
    public string AuthorLastName { get; }
    public string Title { get; }
    public int AvailableCopies { get; }

    private BookDto(Guid id, string authorFirstName, string authorLastName, string title, int availableCopies)
    {
        Id = id;
        AuthorFirstName = authorFirstName;
        AuthorLastName = authorLastName;
        Title = title;
        AvailableCopies = availableCopies;
    }

    public static BookDto FromDomain(Book book) =>
        new(
            book.Id,
            book.Author.Name.FirstName,
            book.Author.Name.LastName,
            book.BookTitle.Value,
            book.BookCopies.Value
        );
}