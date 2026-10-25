using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public class ReturnBookDto(Guid bookId)
{
    public Guid BookId {get;} = bookId;
}