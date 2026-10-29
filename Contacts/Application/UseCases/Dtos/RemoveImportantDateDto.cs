using System;

namespace Application.UseCases.Dtos;

public record RemoveImportantDateDto(
    Guid ContactId,
    string Name
);