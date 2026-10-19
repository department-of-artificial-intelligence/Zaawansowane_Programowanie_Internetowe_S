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
        FirstName firstName,
        LastName lastName,
        Sex sex,
        Age age)
    {
        return Result.Success(new Contact(Guid.NewGuid(), firstName, lastName, sex, age));
    }

    public Result<Unit> Update(FirstName firstName, LastName lastName, Sex sex, Age age)
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
                return Result.Failure<Unit>("Ten adres e-mail nie jest przypisany do tego kontaktu");
            }

            _emails.Remove(emailToRemove);
            return Result.Success(Unit.Instance);
        }
    public bool HasPhone(PhoneNumber number) => _phones.Any(p => p.Number == number);

    public Result<Unit> AddPhone(PhoneNumber number, Guid categoryId)
    {
        if (HasPhone(number))
            return Result.Failure<Unit>("Ten Kontakt ma już numer telefonu");

        return Phone.Create(number, categoryId)
            .Tap(_phones.Add)
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemovePhone(PhoneNumber number)
    {
        var phone = _phones.FirstOrDefault(p => p.Number == number);
        if (phone is null)
            return Result.Failure<Unit>("Nie odnaleziono tego numeru telefonu");

        _phones.Remove(phone);
        return Result.Success(Unit.Instance);
    }
    public bool HasImportantDate(EventName name) => _importantDates.Any(d => d.Name == name);

public Result<Unit> AddImportantDate(EventName name, DateTime date)
{
    if (HasImportantDate(name))
        return Result.Failure<Unit>("Wydarzenia z ta nazwa juz istnieje dla tego kontaktu");

    return ImportantDate.Create(name, date)
        .Tap(_importantDates.Add)
        .Map(_ => Unit.Instance);
}

public Result<Unit> RemoveImportantDate(EventName name)
{
    var dateEntry = _importantDates.FirstOrDefault(d => d.Name == name);
    if (dateEntry is null)
        return Result.Failure<Unit>("Nie odnaleziono tego wydarzenia");

    _importantDates.Remove(dateEntry);
    return Result.Success(Unit.Instance);
}
}