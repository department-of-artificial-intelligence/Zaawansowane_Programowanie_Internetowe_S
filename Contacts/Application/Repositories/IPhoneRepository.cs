using System.Collections.Generic;
using Contacts.Application.Domain;

namespace Contacts.Application.Repositories;

public interface IPhoneRepository
{
    IEnumerable<Phone> GetPhonesByContact(int contactId);
    Phone? GetPhone(int phoneId);

    void Add(Phone phone);
    void Remove(Phone phone);
}