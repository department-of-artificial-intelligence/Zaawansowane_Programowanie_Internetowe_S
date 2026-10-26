
namespace Application.UseCases.Dtos;

public record AddPhoneDto(
    Guid ContactId,
    string Number);
