using Application.Domain;

namespace Application.UseCases.Dtos;

public class AddPhoneDto(Guid contactId, string number, PhoneType type)
{
    public Guid ContactId { get; } = contactId;
    public string Number { get; } = number;
    public PhoneType Type { get; } = type;
}