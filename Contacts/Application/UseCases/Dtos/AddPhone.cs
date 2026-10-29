using System;
namespace Application.UseCases.Dtos;

public record AddPhoneDto(
    Guid ContactId,
    string Number,
    Guid CategoryId
);