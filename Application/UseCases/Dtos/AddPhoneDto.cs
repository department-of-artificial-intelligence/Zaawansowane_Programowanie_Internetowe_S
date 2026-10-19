namespace Application.UseCases.Dtos;

public class AddPhoneDto(Guid contactId, string phoneNumber, Guid categoryId)
{
    public Guid ContactId { get; } = contactId;
    public string PhoneNumber { get; } = phoneNumber;
    public Guid CategoryId { get; } = categoryId;
}