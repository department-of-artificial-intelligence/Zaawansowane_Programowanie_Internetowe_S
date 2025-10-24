using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddEmailResult
{
    public int Id { get; init; }
    public string Address { get; init; } = string.Empty;
    public int ContactId { get; init; }
    public int CategoryId { get; init; }

    public static implicit operator AddEmailResult(Email email)
        => new AddEmailResult
        {
            Id = email.Id,
            Address = email.Address,
            ContactId = email.ContactId,
            CategoryId = email.CategoryId
        };
}