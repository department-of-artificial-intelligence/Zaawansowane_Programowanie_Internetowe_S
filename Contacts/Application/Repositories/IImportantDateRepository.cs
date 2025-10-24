using System.Collections.Generic;
using Contacts.Application.Domain;

namespace Contacts.Application.Repositories;

public interface IImportantDateRepository
{
    IEnumerable<ImportantDate> GetDatesByContact(int contactId);
    ImportantDate? GetDate(int dateId);

    void Add(ImportantDate importantDate);
    void Remove(ImportantDate importantDate);
}