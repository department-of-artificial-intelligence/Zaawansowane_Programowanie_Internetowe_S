namespace Application.UseCases.Dtos;

public record AddBookDto(string Isbn, string Title);
public record RegisterReaderDto(string Email);
public record BorrowBookDto(Guid ReaderId, Guid BookId);