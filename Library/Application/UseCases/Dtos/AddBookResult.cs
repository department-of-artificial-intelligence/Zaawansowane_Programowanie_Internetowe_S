using Application.Domain;

namespace Application.UseCases.Dtos;

public class AddBookResult(Guid id, Author author, BookTitle bookTitle, BookCopies bookCopies)
{
    public Guid Id { get; } = id;
    public Author Author = author;
    public BookTitle BookTitle = bookTitle;
    public BookCopies BookCopies = bookCopies;
    internal static AddBookResult FromDomain(Book book) =>
        new (book.Id, book.Author, book.BookTitle, book.BookCopies);
}