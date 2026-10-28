namespace Application.UseCases.Dtos;
public class AddEmailDto(Guid contactId, string address, Guid categoryId)
{
    public Guid ContactId { get; } = contactId;
    public string Address { get; } = address;
    public Guid CategoryId { get; } = categoryId;
}