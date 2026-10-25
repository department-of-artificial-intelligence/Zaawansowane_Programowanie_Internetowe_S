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
    private Book(Guid id) : base(id)
    {
        Author = null!;
        BookTitle = null!;
        BookCopies = null!;
    }
    public static Result<Book> Create(
        Author author,
        BookTitle bookTitle,
        BookCopies bookCopies)
    {
        return Result.Success(new Book(Guid.NewGuid(), author, bookTitle, bookCopies));
    }
    public Result<Unit> BorrowBook(){
        if(BookCopies.Value <=0){
            return Result.Failure<Unit>("Brak dostępnych ksiązek.");
        }
        return BookCopies.Create(BookCopies.Value - 1)
            .Map(newCopies =>
            {
                BookCopies = newCopies;
                return Unit.Instance;
            });
    }
    public Result<Unit> ReturnBook()
    {
        return BookCopies.Create(BookCopies.Value + 1)
            .Map(newCopies =>
            {
                BookCopies = newCopies;
                return Unit.Instance;
            });
    }
    public Result<Unit> UpdateDetails(Author author, BookTitle bookTitle, BookCopies bookCopies){
        Author = author;
        BookTitle = bookTitle;
        BookCopies = bookCopies;
        return Result.Success(Unit.Instance);
    }
}