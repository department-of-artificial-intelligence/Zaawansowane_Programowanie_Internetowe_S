using Application.Utilities;

namespace Application.Domain;

public class Book : Entity
{

    public Author Author { get; private set; }
    public BookTitle BookTitle { get; private set; }
    public BookCopies BookCopies { get; private set; }

    private Book(Guid id, Author author, BookTitle bookTitle, BookCopies bookCopies) : base(id)
    {
        Author = author;
        BookTitle = bookTitle;
        BookCopies = bookCopies;
    }
    public static Result<Contact> Create(
        Author author,
        BookTitle bookTitle,
        BookCopies bookCopies)
    {
        return Result.Success(new Book(Guid.NewGuid(), author, bookTitle, bookCopies));
    }

}