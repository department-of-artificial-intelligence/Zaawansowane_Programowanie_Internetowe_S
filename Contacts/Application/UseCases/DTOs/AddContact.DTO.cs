using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddContactDTO
{
    public int Id { get; private set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Sex Sex { get; set; }
    public ICollection<Email> Emails { get; set; } = null!;
    public Age Age { get; set; } = null!;

    public static implicit operator Contact(AddContactDTO dto) =>
        new(dto.Id, dto.FirstName, dto.LastName, dto.Sex, dto.Emails, dto.Age);
}

public class EditContactDTO
{
    public int Id { get; private set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Sex Sex { get; set; }
    public ICollection<Email> Emails { get; set; } = null!;
    public Age Age { get; set; } = null!;

    public static implicit operator Contact(EditContactDTO dto) =>
        new(dto.Id, dto.FirstName, dto.LastName, dto.Sex, dto.Emails, dto.Age);
}
