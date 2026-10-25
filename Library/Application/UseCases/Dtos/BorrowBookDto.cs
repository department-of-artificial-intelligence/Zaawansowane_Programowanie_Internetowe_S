using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public class BorrowBookDto(Guid bookId, Guid readerId)
{
    public Guid BookId {get;} = bookId;
    public Guid ReaderId {get;} = readerId;
}