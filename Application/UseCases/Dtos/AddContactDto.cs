using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;
using Application.Utilities;
using DomainAge = Application.Domain.Age;

namespace Application.UseCases.Dtos;

public class AddContactDto(string firstName, string lastName, Sex sex, int age)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public Sex Sex { get; } = sex;
    public int Age { get; } = age;

    public Result<Contact> ToDomain(Guid userId) =>
        Domain.FirstName.Create(FirstName)
            .Bind(fn => Domain.LastName.Create(LastName)
                .Bind(ln => DomainAge.Create(Age)
                    .Bind(ag => Contact.Create(userId, fn, ln, Sex, ag))));
}