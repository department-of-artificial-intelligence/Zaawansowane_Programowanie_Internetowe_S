using System;

namespace Application.UseCases.Dtos;

public record AddBookDto(string Title, string Author, string Isbn);
public record AddBookResult(Guid BookId, string Title);