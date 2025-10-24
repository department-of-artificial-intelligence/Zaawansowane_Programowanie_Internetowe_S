using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddPhoneResult
{
    public int Id { get; init; }
    public string Number { get; init; } = string.Empty;
    public int ContactId { get; init; }
    public int CategoryId { get; init; }

    public static implicit operator AddPhoneResult(Phone phone)
        => new AddPhoneResult
        {
            Id = phone.Id,
            Number = phone.Number,
            ContactId = phone.ContactId,
            CategoryId = phone.CategoryId
        };
}