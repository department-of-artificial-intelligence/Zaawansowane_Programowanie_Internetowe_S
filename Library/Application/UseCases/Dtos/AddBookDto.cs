using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public class AddBookDto(string authorFirstName, string authorLastName, string bookTitle, int bookCopies)
{
    public string AuthorFirstName { get; } = authorFirstName;
    public string AuthorLastName { get; } = authorLastName;
    public string BookTitle { get; } = bookTitle;
    public int BookCopies { get; } = bookCopies;

    public Result<Book> ToDomain() =>
    FullName.Create(AuthorFirstName, AuthorLastName)
            .Bind(fullName => Author.Create(fullName))
            .Bind(author =>
                Domain.BookTitle.Create(BookTitle)
                    .Bind(bookTitle =>
                        Domain.BookCopies.Create(BookCopies)
                            .Bind(bookCopies => 
                                Book.Create(author, bookTitle, bookCopies)
                            )
                    )
            );
}