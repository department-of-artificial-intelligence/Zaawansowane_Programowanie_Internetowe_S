namespace Library.Application.UseCases.Dtos;

public record BorrowBookDto(Guid BookId, Guid ReaderId);