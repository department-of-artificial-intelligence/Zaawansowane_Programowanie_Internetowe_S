using System;

namespace Application.UseCases.Dtos;

public record RemovePhoneDto(
    Guid ContactId,
    string PhoneNumber
);