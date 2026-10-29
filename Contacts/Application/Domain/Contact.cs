using System;
using System.Collections.Generic;
using System.Linq;
using Application.Utilities;


namespace Application.Domain;

public class Contact : Entity
{
    private readonly List<Email> _emails = [];

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Sex Sex { get; private set; }
    public Age Age { get; private set; }
    public IReadOnlyCollection<Email> Emails => _emails;

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

    public static Result<Contact> Create(
        string firstName,
        string lastName,
        Sex sex,
        Age age)
    {
        return FirstName.Create(firstName)
            .Bind(fn => LastName.Create(lastName)
                .Map(ln => new Contact(Guid.NewGuid(), fn, ln, sex, age)));



    }



    //zadanie 5
    public Result<Unit> UpdateDetails(FirstName firstName, LastName lastName, Sex sex, Age age)
    {
        FirstName = firstName;
        LastName = lastName;
        Sex = sex;
        Age = age;
        return Result.Success(Unit.Instance);
    }

    public Result<Unit> RemoveEmail(string emailAddress)
    {
        var emailToRemove = _emails.FirstOrDefault(e => e.Address.Value == emailAddress);

        if (emailToRemove is null)
        {
            return Result.Failure<Unit>($"Nie odnaleziono adresu e-mail '{emailAddress}' w tym kontakcie.");
        }

        _emails.RemoveAll(e => e == emailToRemove);
        return Result.Success(Unit.Instance);
    }

    private readonly List<Phone> _phones = [];
    public IReadOnlyCollection<Phone> Phones => _phones;

    public Result<Unit> AddPhone(string number, Guid categoryId)
    {
        return Phone.Create(number, categoryId)
            .Tap(phone => _phones.Add(phone))
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemovePhone(string phoneNumber)
    {
        var phoneToRemove = _phones.FirstOrDefault(p => p.Number.Value == phoneNumber);

        if (phoneToRemove is null)
        {
            return Result.Failure<Unit>($"Nie odnaleziono numeru telefonu '{phoneNumber}' w tym kontakcie.");
        }

        _phones.Remove(phoneToRemove);
        return Result.Success(Unit.Instance);
    }

    // Wewnątrz klasy Contact w pliku Contact.cs dodaj:

    private readonly List<ContactDate> _importantDates = [];
    public IReadOnlyCollection<ContactDate> ImportantDates => _importantDates;

    public Result<Unit> AddImportantDate(string name, DateTime date)
    {
        return ContactDate.Create(name, date)
            .Tap(contactDate => _importantDates.Add(contactDate))
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemoveImportantDate(string name)
    {
        var dateToRemove = _importantDates.FirstOrDefault(d => d.ImportantDate.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (dateToRemove is null)
        {
            return Result.Failure<Unit>($"Nie odnaleziono ważnej daty o nazwie '{name}' dla tego kontaktu.");
        }

        _importantDates.Remove(dateToRemove);
        return Result.Success(Unit.Instance);
    }


}