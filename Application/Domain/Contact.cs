using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Utilities;

namespace Application.Domain;

public class Contact : Entity
{
    private readonly List<Email> _emails = [];
    private readonly List<PhoneNumber> _phones = [];
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Sex Sex { get; private set; }
    public Age Age { get; private set; }
    public IReadOnlyCollection<Email> Emails => _emails;
    public IReadOnlyCollection<PhoneNumber> Phones => _phones.AsReadOnly();

    private Contact() : base(Guid.Empty)
    {
        FirstName = null!;
        LastName = null!;
        Age = null!;
    }
    private Contact(
        Guid id,
        FirstName firstName,
        LastName lastName,
        Sex sex,
        Age age)
        : this(id, firstName, lastName, sex)
    {
        Age = age;
    }
    private Contact(
        Guid id,
        FirstName firstName,
        LastName lastName,
        Sex sex) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Sex = sex;
        Age = null!;
    }

    public bool HasEmail(EmailAddress address) =>
    _emails.Any(e => e.Address == address);
    public bool HasPhone(PhoneNumber number) =>
    _phones.Contains(number);

    public Result<Unit> AddEmail(EmailAddress address, Guid categoryId)
    {
        if (HasEmail(address))
        {
            return Result.Failure<Unit>("Email już istnieje w kontakcie");
        }
        return Email
        .Create(address, categoryId)
        .Tap(_emails.Add)
        .Map(_ => Unit.Instance);
    }
    public Result<Unit> AddPhone(PhoneNumber number)
    {
        if (HasPhone(number))
        {
            return Result.Failure<Unit>("Ten numer telefonu jest już przypisany do kontaktu.");
        }

        _phones.Add(number);
        return Result.Success(Unit.Instance);
    }
    public static Result<Contact> Create(
        FirstName firstName,
        LastName lastName,
        Sex sex,
        Age age)
    {
        var contact = new Contact(Guid.NewGuid(), firstName, lastName, sex, age);
        return Result.Success(contact);
    }

    public Result<Unit> UpdateDetails(FirstName firstName, LastName lastName, Sex sex, Age age)
    {
        FirstName = firstName;
        LastName = lastName;
        Sex = sex;
        Age = age;
        return Result.Success(Unit.Instance);
    }

    public Result<Unit> RemoveEmail(EmailAddress address)
    {
        var emailToRemove = _emails.FirstOrDefault(e => e.Address == address);
        if (emailToRemove is null)
        {
            return Result.Failure<Unit>("Podany adres e-mail nie istnieje w tym kontakcie.");
        }

        _emails.Remove(emailToRemove);
        return Result.Success(Unit.Instance);
    }
    public Result<Unit> RemovePhone(PhoneNumber number)
    {
        var phoneToRemove = _phones.FirstOrDefault(p => p.Value == number.Value);
        if (phoneToRemove is null)
        {
            return Result.Failure<Unit>("Podany numer telefonu nie istnieje w tym kontakcie.");
        }

        _phones.Remove(phoneToRemove);
        return Result.Success(Unit.Instance);
    }
}

