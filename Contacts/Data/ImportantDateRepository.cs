using System.Collections.Generic;
using System.Linq;
using Contacts.Application.Domain;
using Contacts.Application.Repositories;

namespace Contacts.Data;

public class ImportantDateRepository(AppDbContext context) : IImportantDateRepository
{
    public IEnumerable<ImportantDate> GetDatesByContact(int contactId)
        => context.ImportantDates.Where(d => d.ContactId == contactId);

    public ImportantDate? GetDate(int dateId)
        => context.ImportantDates.Find(dateId);

    public void Add(ImportantDate importantDate)
    {
        context.ImportantDates.Add(importantDate);
    }

    public void Remove(ImportantDate importantDate)
    {
        context.ImportantDates.Remove(importantDate);
    }
}