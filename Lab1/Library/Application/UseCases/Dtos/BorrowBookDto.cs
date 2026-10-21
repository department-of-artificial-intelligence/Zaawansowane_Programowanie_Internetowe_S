namespace Application.UseCases.Dtos;

public class BorrowBookDto(Guid readerId, Guid bookId)
{
    public Guid ReaderId { get; } = readerId;
    public Guid BookId { get; } = bookId;
}
