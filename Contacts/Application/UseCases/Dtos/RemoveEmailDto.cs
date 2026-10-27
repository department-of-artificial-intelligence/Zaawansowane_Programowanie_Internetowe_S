namespace Application.UseCases.Dtos;

public record RemoveEmailDto(
    Guid ContactId,
    string EmailAddress
);