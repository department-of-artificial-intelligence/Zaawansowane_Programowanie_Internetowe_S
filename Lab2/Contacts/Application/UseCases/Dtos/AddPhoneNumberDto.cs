namespace Application.UseCases.Dtos;

public class AddPhoneNumberDto(Guid contactId, string number)
{
    public Guid ContactId { get; } = contactId;
    public string Number { get; } = number;
}
