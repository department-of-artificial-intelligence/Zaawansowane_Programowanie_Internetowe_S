using Application.Queries.Dtos;
using Application.Utilities;
namespace Application.Queries;

public interface IContactQueries
{
    IEnumerable<ContactSummaryDto> GetContacts();
    Option<ContactDetailsDto> GetContact(Guid id);
}
