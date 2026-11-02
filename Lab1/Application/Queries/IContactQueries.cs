using Application.Queries.Dtos;
using Application.Utilities;
namespace Application.Queries;

public interface IContactQueries
{
    IEnumerable<ContactSummaryDto> GetContacts(Guid userId);
    Option<ContactDetailsDto> GetContact(Guid id, Guid userId);
}
