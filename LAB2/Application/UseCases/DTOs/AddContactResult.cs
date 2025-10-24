using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddContactResult
{
    public int Id { get; init; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ICollection<Email> Emails { get; set; } = new List<Email>();
    public Sex Sex { get; set; }
    public Age Age { get; set; }

    public static implicit operator AddContactResult(Contact dto) =>
        new AddContactResult{
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Sex = dto.Sex,
            Emails = dto.Emails,
            Age = dto.Age
        };
}
