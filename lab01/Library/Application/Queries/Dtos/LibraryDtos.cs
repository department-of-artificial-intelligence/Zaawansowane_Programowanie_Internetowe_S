namespace Application.Queries.Dtos;

public record BookDto(Guid Id, string Isbn, string Title, string Author, int Copies, int AvailableCopies);

public record LoanDto(Guid Id, string BookTitle, string ReaderName, DateOnly LoanDate, DateOnly DueDate);
