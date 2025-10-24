using System.Collections.Generic;
using System.Linq;
using Contacts.Application.Domain;
using Contacts.Application.Repositories;

namespace Contacts.Data;

public class PhoneRepository(AppDbContext context) : IPhoneRepository
{
    public IEnumerable<Phone> GetPhonesByContact(int contactId)
        => context.Phones.Where(p => p.ContactId == contactId);

    public Phone? GetPhone(int phoneId)
        => context.Phones.Find(phoneId);

    public void Add(Phone phone)
    {
        context.Phones.Add(phone);
    }

    public void Remove(Phone phone)
    {
        context.Phones.Remove(phone);
    }
}