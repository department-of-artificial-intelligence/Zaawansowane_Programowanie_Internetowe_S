using System;
using Application.Utilities;

namespace Application.Domain;

public class ContactDate : Entity
{
    public ImportantDate ImportantDate { get; private set; }

    private ContactDate() : base(Guid.NewGuid())
    {
        ImportantDate = null!;
    }

    private ContactDate(ImportantDate importantDate) : base(Guid.NewGuid())
    {
        ImportantDate = importantDate;
    }

    public static Result<ContactDate> Create(string name, DateTime date)
    {
        return ImportantDate.Create(name, date)
            .Map(impDate => new ContactDate(impDate));
    }
}