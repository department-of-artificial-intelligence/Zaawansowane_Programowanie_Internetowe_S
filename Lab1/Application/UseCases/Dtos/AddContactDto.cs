using Application.Domain;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos
{
    public class AddContactDto(string firstName, string lastName, Sex sex, int age, Guid UserId)
    {
        public string FirstName { get; } = firstName;
        public string LastName { get; } = lastName;
        public Sex Sex { get; } = sex;
        public int Age { get; } = age;

        public Result<Contact> ToDomain() =>
            Domain.FirstName.Create(FirstName)
                .Bind(fn => Domain.LastName.Create(LastName)
                .Bind(ln => Domain.Age.Create(Age)
                .Bind(age => Contact.Create(fn, ln, Sex, age, UserId))));
    }



}
