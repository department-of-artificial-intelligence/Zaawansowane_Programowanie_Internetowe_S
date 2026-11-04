namespace Application.UseCases.Dtos;

public class PhoneNumberDto(Guid contactId, string number)
{
    public Guid ContactId { get; } = contactId;
    public string Number { get; } = number;
}
