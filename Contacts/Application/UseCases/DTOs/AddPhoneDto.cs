namespace Application.UseCases.Dtos;

public class AddPhoneDto(Guid contactId, string number, Guid categoryId)
{
    public Guid ContactId { get; } = contactId;
    public string Number { get; } = number;
    public Guid CategoryId { get; } = categoryId;
}