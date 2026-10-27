using Application.Utilities;

namespace Application.Domain;

public class Contact : Entity
{
    private readonly List<Email> _emails = [];

    private readonly List<Phone> _phones = [];

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Sex Sex { get; private set; }
    public Age Age { get; private set; }

    public IReadOnlyCollection<Email> Emails => _emails;

    public IReadOnlyCollection<Phone> Phones => _phones;
    private readonly List<ImportantDate> _importantDates = [];
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
            return Result.Conflict<Unit>("Email już istnieje w kontakcie");
        }

        return Email
            .Create(address, categoryId)
            .Tap(_emails.Add)
            .Map(_ => Unit.Instance);
    }

    public static Result<Contact> Create(
    string firstName, string lastName, Sex sex, Age age) =>
    FirstName.Create(firstName)
        .Bind(fn => LastName.Create(lastName)
            .Map(ln => new Contact(Guid.NewGuid(), fn, ln, sex, age)));
    
    public Result<Unit> Update(string firstName, string lastName, Sex sex, Age age) =>
    FirstName.Create(firstName)
        .Bind(fn => LastName.Create(lastName).Map(ln => (fn, ln)))
        .Tap(names =>
        {
            FirstName = names.fn;
            LastName = names.ln;
            Sex = sex;
            Age = age;
        })
        .Map(_ => Unit.Instance);

public Result<Unit> RemoveEmail(EmailAddress address)
{
    var email = _emails.SingleOrDefault(e => e.Address == address);
    if (email is null)
    {
        return Result.NotFound<Unit>($"Kontakt nie ma adresu {address}.");
    }

    _emails.Remove(email);
    return Result.Success(Unit.Instance);
}
public Result<Unit> AddPhone(PhoneNumber number, PhoneType type)
{
    if (_phones.Any(p => p.Number == number))
    {
        return Result.Conflict<Unit>($"Numer {number} już istnieje w kontakcie.");
    }

    _phones.Add(Phone.Create(number, type));
    return Result.Success(Unit.Instance);
}

public Result<Unit> RemovePhone(PhoneNumber number)
{
    var phone = _phones.SingleOrDefault(p => p.Number == number);
    if (phone is null)
    {
        return Result.NotFound<Unit>($"Kontakt nie ma numeru {number}.");
    }

    _phones.Remove(phone);
    return Result.Success(Unit.Instance);
}
public Result<Guid> AddImportantDate(DateLabel label, DateOnly date)
{
    if (_importantDates.Any(d => d.Label == label))
    {
        return Result.Conflict<Guid>($"Kontakt ma już datę \"{label}\".");
    }

    var importantDate = ImportantDate.Create(label, date);
    _importantDates.Add(importantDate);
    return Result.Success(importantDate.Id);
}

public Result<Unit> ChangeImportantDate(Guid id, DateLabel label, DateOnly date) =>
    FindImportantDate(id)
        .Bind(importantDate =>
            _importantDates.Any(d => d.Id != id && d.Label == label)
                ? Result.Conflict<ImportantDate>($"Kontakt ma już datę \"{label}\".")
                : Result.Success(importantDate))
        .Tap(importantDate => importantDate.Change(label, date))
        .Map(_ => Unit.Instance);

public Result<Unit> RemoveImportantDate(Guid id) =>
    FindImportantDate(id)
        .Tap(importantDate => _importantDates.Remove(importantDate))
        .Map(_ => Unit.Instance);

private Result<ImportantDate> FindImportantDate(Guid id)
{
    var importantDate = _importantDates.SingleOrDefault(d => d.Id == id);
    return importantDate is not null
        ? Result.Success(importantDate)
        : Result.NotFound<ImportantDate>($"Kontakt nie ma daty o id = {id}.");
}
}