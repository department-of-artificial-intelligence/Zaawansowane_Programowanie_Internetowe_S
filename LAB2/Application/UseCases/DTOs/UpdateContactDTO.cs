using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class UpdateContactDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ICollection<Email> Emails { get; set; }
    public Sex Sex { get; set; }
    public Age Age { get; set; }

    public static implicit operator Contact(UpdateContactDTO dto) =>
        new(dto.Id, dto.FirstName, dto.LastName, dto.Sex, dto.Emails, dto.Age);
}
