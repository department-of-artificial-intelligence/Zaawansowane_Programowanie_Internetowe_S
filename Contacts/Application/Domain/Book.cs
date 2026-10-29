using System;
using Application.Utilities;

namespace Application.Domain;

public enum BookStatus
{
    Available,
    Lent,
    Reserved
}

public class Book : Entity
{
    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public ISBN Isbn { get; private set; } = null!;
    public BookStatus Status { get; private set; }

    private Book() : base(Guid.Empty) { }

    private Book(string title, string author, ISBN isbn) : base(Guid.NewGuid())
    {
        Title = title;
        Author = author;
        Isbn = isbn;
        Status = BookStatus.Available;
    }

    public static Result<Book> Create(string title, string author, string isbn)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Book>("Tytuł nie może być pusty.");

        if (string.IsNullOrWhiteSpace(author))
            return Result.Failure<Book>("Autor nie może być pusty.");

        return ISBN.Create(isbn)
            .Map(validIsbn => new Book(title.Trim(), author.Trim(), validIsbn));
    }

    public Result<Unit> MarkAsLent()
    {
        if (Status != BookStatus.Available)
        {
            return Result.Failure<Unit>("Książka nie jest dostępna do wypożyczenia.");
        }

        Status = BookStatus.Lent;
        return Result.Success(Unit.Instance);
    }

    public Result<Unit> MarkAsAvailable()
    {
        Status = BookStatus.Available;
        return Result.Success(Unit.Instance);
    }
}