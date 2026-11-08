namespace Application.UseCases.Dtos;

public record RemovePhoneNumberDto(
    Guid ContactId,
    string PhoneNumber
);