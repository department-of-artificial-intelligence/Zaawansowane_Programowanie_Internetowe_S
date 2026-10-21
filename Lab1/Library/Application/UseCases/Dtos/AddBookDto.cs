using Application.Domain;
using Application.Utilities;
using DomainIsbn = Application.Domain.Isbn;

namespace Application.UseCases.Dtos;

public class AddBookDto(string title, string author, string isbn, int totalCopies)
{
    public string Title { get; } = title;
    public string Author { get; } = author;
    public string Isbn { get; } = isbn;
    public int TotalCopies { get; } = totalCopies;

    public Result<Book> ToDomain() =>
        DomainIsbn.Create(Isbn)
            .Bind(parsedIsbn => Book.Create(Title, Author, parsedIsbn, TotalCopies));
}
