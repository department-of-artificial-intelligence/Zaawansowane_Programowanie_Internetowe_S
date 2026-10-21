using Application.Utilities;

namespace Application.Domain;

public class Book : Entity
{
    public Isbn Isbn { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public int Copies { get; private set; }

    private Book(Guid id, Isbn isbn, string title, string author, int copies)
        : this(id, title, author, copies)
    {
        Isbn = isbn;
    }

    // Konstruktor wykorzystywany przez Entity Framework.
    private Book(Guid id, string title, string author, int copies) : base(id)
    {
        Isbn = null!;
        Title = title;
        Author = author;
        Copies = copies;
    }

    public static Result<Book> Create(Isbn isbn, string title, string author, int copies) =>
        Result.Combine(
                Validators.NotEmpty(title, "Tytuł jest wymagany."),
                Validators.NotEmpty(author, "Autor jest wymagany."),
                Validators.InRange(copies, 1, 1000, "Liczba egzemplarzy musi być z zakresu 1-1000."))
            .Map(_ => new Book(Guid.NewGuid(), isbn, title.Trim(), author.Trim(), copies));
}
