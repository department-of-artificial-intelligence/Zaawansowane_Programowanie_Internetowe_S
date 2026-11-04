using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;

namespace Application.UseCases.Dtos;

public class AddContactResult(
Guid id, string firstName, string lastName, Sex sex, int age
)
{
    public Guid Id { get; } = id;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public Sex Sex { get; } = sex;
    public int Age { get; } = age;
    internal static AddContactResult FromDomain(Contact contact) =>
        new(contact.Id,
            contact.FirstName.Value, // <-- Pobranie wartości z obiektu FirstName
            contact.LastName.Value,  // <-- Pobranie wartości z obiektu LastName
            contact.Sex,
            contact.Age.Value);
}
