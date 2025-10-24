using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddContactDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ICollection<Email> Emails { get; set; } = new List<Email>();
    public Sex Sex { get; set; }
    public Age Age { get; set; }

    public static implicit operator Contact(AddContactDTO dto) =>
        new(dto.FirstName, dto.LastName, dto.Sex, dto.Emails, dto.Age);
}
