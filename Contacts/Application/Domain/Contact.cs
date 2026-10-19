using Application.Utilities;

namespace Application.Domain;

public class Contact : Entity
{
    private readonly List<Email> _emails = [];
    private readonly List<Phone> _phones = [];
    private readonly List<ImportantDate> _importantDates = [];

    public FirstName FirstName { get; private set; } = null!;
    public LastName LastName { get; private set; } = null!;
    public Sex Sex { get; private set; }
    public Age Age { get; private set; } = null!;
    public IReadOnlyCollection<Email> Emails => _emails;
    public IReadOnlyCollection<Phone> Phones => _phones;
    public IReadOnlyCollection<ImportantDate> ImportantDates => _importantDates;

    private Contact() : base(Guid.Empty) { }

    private Contact(Guid id, FirstName firstName, LastName lastName, Sex sex, Age age) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Sex = sex;
        Age = age;
    }

    public bool HasEmail(EmailAddress address) =>
        _emails.Any(e => e.Address == address);

    public Result<Unit> AddEmail(EmailAddress address, Guid categoryId)
    {
        if (HasEmail(address))
        {
            return Result.Failure<Unit>("Email już istnieje w kontakcie");
        }

        return Email.Create(address, categoryId)
            .Tap(_emails.Add)
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemoveEmail(EmailAddress address)
    {
        var emailToRemove = _emails.FirstOrDefault(e => e.Address == address);
        if (emailToRemove is null)
        {
            return Result.Failure<Unit>("Podany adres email nie istnieje w tym kontakcie.");
        }

        _emails.Remove(emailToRemove);
        return Result.Success(Unit.Instance);
    }

    public bool HasPhone(PhoneNumber number) =>
        _phones.Any(p => p.Number == number);

    public Result<Unit> AddPhone(PhoneNumber number, Guid categoryId)
    {
        if (HasPhone(number))
        {
            return Result.Failure<Unit>("Numer telefonu już istnieje w tym kontakcie.");
        }

        return Phone.Create(number, categoryId)
            .Tap(_phones.Add)
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemovePhone(PhoneNumber number)
    {
        var phoneToRemove = _phones.FirstOrDefault(p => p.Number == number);
        if (phoneToRemove is null)
        {
            return Result.Failure<Unit>("Podany numer telefonu nie istnieje w tym kontakcie.");
        }

        _phones.Remove(phoneToRemove);
        return Result.Success(Unit.Instance);
    }

    // Obsługa ważnych dat (Zadanie 8)
    public Result<Unit> AddImportantDate(ImportantDate importantDate)
    {
        if (_importantDates.Any(d => d.Description.Equals(importantDate.Description, StringComparison.OrdinalIgnoreCase) && d.Date == importantDate.Date))
        {
            return Result.Failure<Unit>("Taka data z tym opisem już istnieje w tym kontakcie.");
        }

        _importantDates.Add(importantDate);
        return Result.Success(Unit.Instance);
    }

    public Result<Unit> RemoveImportantDate(string description, DateOnly date)
    {
        var item = _importantDates.FirstOrDefault(d => d.Description.Equals(description, StringComparison.OrdinalIgnoreCase) && d.Date == date);
        if (item is null)
        {
            return Result.Failure<Unit>("Nie znaleziono takiej daty w kontakcie.");
        }

        _importantDates.Remove(item);
        return Result.Success(Unit.Instance);
    }

    public Result<Unit> Update(FirstName firstName, LastName lastName, Sex sex, Age age)
    {
        FirstName = firstName;
        LastName = lastName;
        Sex = sex;
        Age = age;
        return Result.Success(Unit.Instance);
    }

    public static Result<Contact> Create(FirstName firstName, LastName lastName, Sex sex, Age age) =>
        Result.Success(new Contact(Guid.NewGuid(), firstName, lastName, sex, age));
}