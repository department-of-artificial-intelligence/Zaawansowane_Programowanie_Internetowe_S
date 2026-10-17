using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;
using Application.Utilities;
using DomainAge = Application.Domain.Age;

namespace Application.UseCases.Dtos;

public class UpdateContactDto(Guid id, string firstName, string lastName, Sex sex, int age)
{
    public Guid Id { get; } = id;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public Sex Sex { get; } = sex;
    public int Age { get; } = age;

    public Result<(Domain.FirstName FirstName, Domain.LastName LastName, Domain.Age Age)> ToDomainValues() =>
        Domain.FirstName.Create(FirstName)
            .Bind(fn => Domain.LastName.Create(LastName)
                .Bind(ln => DomainAge.Create(Age)
                    .Map(ag => (fn, ln, ag))));
}
