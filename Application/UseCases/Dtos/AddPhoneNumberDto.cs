namespace Application.UseCases.Dtos;

public class AddPhoneNumberDto(Guid contactId, string phoneNumber)
{
    public Guid ContactId { get; } = contactId;
    public string Number { get; } = phoneNumber;
}
