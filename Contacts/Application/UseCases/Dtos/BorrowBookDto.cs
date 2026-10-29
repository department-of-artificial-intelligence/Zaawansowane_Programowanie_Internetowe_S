using System;

namespace Application.UseCases.Dtos;

public record BorrowBookDto(Guid BookId, Guid ReaderId);