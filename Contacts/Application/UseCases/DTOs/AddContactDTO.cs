using System.Collections.Generic;
using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddContactDTO
{
    public string Firstname { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public Sex Sex { get; init; }
    public int Age { get; init; }

    public static implicit operator Contact(AddContactDTO dto)
        => new Contact(0, dto.Firstname, dto.LastName, dto.Sex, new List<Email>(), new Age(dto.Age));
}