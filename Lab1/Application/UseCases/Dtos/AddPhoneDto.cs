using Application.Domain;
namespace Application.UseCases.Dtos;
public record AddPhoneDto(Guid ContactId, string Number, PhoneType Type);