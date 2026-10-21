namespace Application.UseCases.Dtos;

public class RemovePhoneDto(Guid contactId, string phoneNumber)
{
    public Guid ContactId { get; } = contactId;
    public string PhoneNumber { get; } = phoneNumber;
}