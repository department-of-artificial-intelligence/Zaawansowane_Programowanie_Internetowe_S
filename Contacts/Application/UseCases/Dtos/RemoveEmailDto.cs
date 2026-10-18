namespace Application.UseCases.Dtos;

public class RemoveEmailDto(Guid contactId, string emailAddress)
{
    public Guid ContactId { get; } = contactId;
    public string EmailAddress { get; } = emailAddress;
}