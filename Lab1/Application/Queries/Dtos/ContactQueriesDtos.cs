using Application.Domain;
namespace Application.Queries.Dtos;

/// <summary>Kontakt na liście (bez e-maili, telefonów i dat).</summary>
public record ContactSummaryDto(Guid Id, string FirstName, string LastName, Sex Sex, int Age)
{
    public static ContactSummaryDto FromDomain(Contact contact) =>
        new(contact.Id, contact.FirstName.Value, contact.LastName.Value,
            contact.Sex, contact.Age.Value);
}

public record EmailDto(string Address, Guid CategoryId);

public record PhoneDto(string Number, PhoneType Type);

public record ImportantDateDto(Guid Id, string Label, DateOnly Date);

/// <summary>Pełne dane jednego kontaktu.</summary>
public record ContactDetailsDto(
    Guid Id,
    string FirstName,
    string LastName,
    Sex Sex,
    int Age,
    IReadOnlyList<EmailDto> Emails,
    IReadOnlyList<PhoneDto> Phones,
    IReadOnlyList<ImportantDateDto> ImportantDates)
{
    public static ContactDetailsDto FromDomain(Contact contact) =>
        new(contact.Id,
            contact.FirstName.Value,
            contact.LastName.Value,
            contact.Sex,
            contact.Age.Value,
            contact.Emails
                .Select(e => new EmailDto(e.Address.Value, e.CategoryId))
                .ToList(),
            contact.Phones
                .Select(p => new PhoneDto(p.Number.Value, p.Type))
                .ToList(),
            contact.ImportantDates
                .OrderBy(d => d.Date)
                .Select(d => new ImportantDateDto(d.Id, d.Label.Value, d.Date))
                .ToList());
}
