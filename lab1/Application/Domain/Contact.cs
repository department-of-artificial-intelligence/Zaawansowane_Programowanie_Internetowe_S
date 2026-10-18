
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
    private readonly List<Phone> _phones = [];
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

    private Contact() : base(Guid.Empty)
{
    FirstName = null!;
    LastName = null!;
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

    
public Result<Unit> RemoveEmail(EmailAddress address)
{
    var email = _emails.FirstOrDefault(e => e.Address == address);

    if (email is null)
    {
        return Result.Failure<Unit>(
            "Podany adres e-mail nie istnieje w kontakcie.");
    }

    _emails.Remove(email);

    return Result.Success(Unit.Instance);
}


    
    public Result<Unit> Update(
        string firstName,
        string lastName,
        Sex sex,
        Age age)
    {
        return FirstName.Create(firstName)
            .Bind(first => LastName.Create(lastName)
                .Map(last =>
                {
                    FirstName = first;
                    LastName = last;
                    Sex = sex;
                    Age = age;

                    return Unit.Instance;
                }));
    }


    public static Result<Contact> Create(
        string firstName,
        string lastName,
        Sex sex,
        Age age)
    {
        return FirstName.Create(firstName)
            .Bind(first => LastName.Create(lastName)
                .Map(last => new Contact(
                    Guid.NewGuid(), first, last, sex, age)));
    }


    
    public Result<Unit> AddPhone(PhoneNumber number)
    {
        if (_phones.Any(p => p.Number == number))
            return Result.Failure<Unit>(
                "Ten numer telefonu jest już przypisany do kontaktu.");

        return Phone.Create(number)
            .Tap(_phones.Add)
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemovePhone(PhoneNumber number)
    {
        var phone = _phones.FirstOrDefault(p => p.Number == number);

        if (phone is null)
            return Result.Failure<Unit>(
                "Podany numer telefonu nie istnieje w kontakcie.");

        _phones.Remove(phone);

        return Result.Success(Unit.Instance);
    }

    
    public Result<Unit> AddImportantDate(EventName name, DateOnly date)
    {
        if (_importantDates.Any(d => d.Name == name && d.Date == date))
        {
            return Result.Failure<Unit>(
                "Takie wydarzenie jest już przypisane do kontaktu.");
        }

        return ImportantDate.Create(name, date)
            .Tap(_importantDates.Add)
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemoveImportantDate(Guid importantDateId)
    {
        var importantDate = _importantDates
            .FirstOrDefault(d => d.Id == importantDateId);

        if (importantDate is null)
        {
            return Result.Failure<Unit>(
                "Nie znaleziono wskazanej ważnej daty.");
        }

        _importantDates.Remove(importantDate);

        return Result.Success(Unit.Instance);
    }


}
