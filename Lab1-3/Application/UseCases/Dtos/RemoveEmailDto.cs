namespace Application.UseCases.Dtos;

public class RemoveEmailDto(Guid contactId, string address)
{
    public Guid ContactId { get; } = contactId;
    public string Address { get; } = address;
}