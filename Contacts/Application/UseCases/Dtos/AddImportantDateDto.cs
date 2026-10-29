using System;

namespace Application.UseCases.Dtos;

public record AddImportantDateDto(
    Guid ContactId,
    string Name,
    DateTime Date
);