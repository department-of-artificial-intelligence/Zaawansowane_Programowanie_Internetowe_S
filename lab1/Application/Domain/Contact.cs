
using Application.Utilities;

namespace Application.Domain;

public class Contact : Entity
{
    private readonly List<Email> _emails = [];
    private readonly List<Phone> _phones = [];
    private readonly List<ImportantDate> _importantDates = [];

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Sex Sex { get; private set; }
    public Age Age { get; private set; }

    public IReadOnlyCollection<Email> Emails => _emails;
    public IReadOnlyCollection<Phone> Phones => _phones;
    public IReadOnlyCollection<ImportantDate> ImportantDates => _importantDates;

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

    public Result<Unit> AddEmail(
        EmailAddress address,
        Guid categoryId)
    {
        if (HasEmail(address))
        {
            return Result.Failure<Unit>(
                "Email już istnieje w kontakcie");
        }

        return Email
            .Create(address, categoryId)
            .Tap(_emails.Add)
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemoveEmail(EmailAddress address)
    {
        var email = _emails.FirstOrDefault(
            e => e.Address == address);

        if (email is null)
        {
            return Result.Failure<Unit>(
                "Nie znaleziono adresu e-mail w kontakcie.");
        }

        _emails.Remove(email);

        return Result.Success(Unit.Instance);
    }

    public Result<Unit> AddPhone(PhoneNumber number)
    {
        if (_phones.Any(phone => phone.Number == number))
        {
            return Result.Failure<Unit>(
                "Ten numer telefonu już istnieje w kontakcie.");
        }

        return Phone
            .Create(number)
            .Tap(_phones.Add)
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemovePhone(PhoneNumber number)
    {
        var phone = _phones.FirstOrDefault(
            p => p.Number == number);

        if (phone is null)
        {
            return Result.Failure<Unit>(
                "Nie znaleziono numeru telefonu w kontakcie.");
        }

        _phones.Remove(phone);

        return Result.Success(Unit.Instance);
    }

    public Result<Unit> AddImportantDate(
        string name,
        DateOnly date)
    {
        if (_importantDates.Any(
            item => string.Equals(
                item.Name,
                name?.Trim(),
                StringComparison.OrdinalIgnoreCase)
            && item.Date == date))
        {
            return Result.Failure<Unit>(
                "Taka ważna data już istnieje w kontakcie.");
        }

        return ImportantDate
            .Create(name, date)
            .Tap(_importantDates.Add)
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemoveImportantDate(
        string name,
        DateOnly date)
    {
        var importantDate = _importantDates.FirstOrDefault(
            item => string.Equals(
                item.Name,
                name?.Trim(),
                StringComparison.OrdinalIgnoreCase)
            && item.Date == date);

        if (importantDate is null)
        {
            return Result.Failure<Unit>(
                "Nie znaleziono ważnej daty w kontakcie.");
        }

        _importantDates.Remove(importantDate);

        return Result.Success(Unit.Instance);
    }

    public Result<Unit> Update(
        string firstName,
        string lastName,
        Sex sex,
        Age age)
    {
        return FirstName.Create(firstName)
            .Bind(newFirstName =>
                LastName.Create(lastName)
                    .Map(newLastName => (
                        FirstName: newFirstName,
                        LastName: newLastName
                    ))
            )
            .Tap(names =>
            {
                FirstName = names.FirstName;
                LastName = names.LastName;
                Sex = sex;
                Age = age;
            })
            .Map(_ => Unit.Instance);
    }

    public static Result<Contact> Create(
        string firstName,
        string lastName,
        Sex sex,
        Age age)
    {
        return FirstName.Create(firstName)
            .Bind(first =>
                LastName.Create(lastName)
                    .Map(last =>
                        new Contact(
                            Guid.NewGuid(),
                            first,
                            last,
                            sex,
                            age
                        )
                    )
            );
    }
}
