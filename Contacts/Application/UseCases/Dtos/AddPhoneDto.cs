namespace Application.UseCases.Dtos;

public class AddPhoneDto(Guid contactId, string number)
{
    public Guid ContactId { get; } = contactId;
    public string PhoneNumber{ get; } = number;
}