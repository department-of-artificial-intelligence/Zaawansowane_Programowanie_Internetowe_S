namespace Application.UseCases.Dtos;

public class UpdateBookDto(Guid id, string title, string author, int totalCopies)
{
    public Guid Id { get; } = id;
    public string Title { get; } = title;
    public string Author { get; } = author;
    public int TotalCopies { get; } = totalCopies;
}
