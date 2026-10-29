using System;

namespace Application.UseCases.Dtos;

public record RegisterReaderDto(string FirstName, string LastName, string Email);
public record RegisterReaderResult(Guid ReaderId, string FullName);