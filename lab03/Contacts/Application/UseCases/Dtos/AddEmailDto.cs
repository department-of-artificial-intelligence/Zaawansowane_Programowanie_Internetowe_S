namespace Application.UseCases.Dtos;

public class AddEmailDto(Guid userId, Guid contactId, string address, Guid categoryId)
{
    public Guid UserId { get; } = userId;
    public Guid ContactId { get; } = contactId;
    public string Address { get; } = address;
    public Guid CategoryId { get; } = categoryId;
}
