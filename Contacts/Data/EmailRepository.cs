using System.Collections.Generic;
using System.Linq;
using Contacts.Application.Domain;
using Contacts.Application.Repositories;

namespace Contacts.Data;

public class EmailRepository(AppDbContext context) : IEmailRepository
{
    public IEnumerable<Email> GetEmailsByContact(int contactId)
        => context.Emails.Where(e => e.ContactId == contactId);

    public Email? GetEmail(int emailId)
        => context.Emails.Find(emailId);

    public void Add(Email email)
    {
        context.Emails.Add(email);
    }

    public void Remove(Email email)
    {
        context.Emails.Remove(email);
    }
}