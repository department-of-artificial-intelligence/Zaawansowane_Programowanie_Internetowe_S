using Application.Domain;
namespace Application.UseCases.Dtos;

public class UpdateBookDto(Guid id, string authorFirstName, string authorLastName, string bookTitle, int bookCopies)
{
    public Guid Id { get; } = id;
    public string AuthorFirstName { get; } = authorFirstName;
    public string AuthorLastName { get; } = authorLastName;
    public string BookTitle { get; } = bookTitle;
    public int BookCopies { get; } = bookCopies;
}