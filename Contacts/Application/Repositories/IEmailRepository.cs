using System.Collections.Generic;
using Contacts.Application.Domain;

namespace Contacts.Application.Repositories;

public interface IEmailRepository
{
    IEnumerable<Email> GetEmailsByContact(int contactId);
    Email? GetEmail(int emailId);

    void Add(Email email);
    void Remove(Email email);
}