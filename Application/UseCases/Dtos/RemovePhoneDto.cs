namespace Application.UseCases.Dtos;

public class RemovePhoneDto(Guid contactId, string number)
{
    public Guid ContactId { get; } = contactId;
    public string Number { get; } = number;
}