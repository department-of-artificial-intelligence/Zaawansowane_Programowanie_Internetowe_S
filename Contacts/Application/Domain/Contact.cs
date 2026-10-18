using Application.Utilities;

namespace Application.Domain;

public class Contact : Entity
{
    private readonly List<Email> _emails = [];
    private readonly List<PhoneNumber> _numbers = [];
    private readonly List<ImportantDate> _dates = [];

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Sex Sex { get; private set; }
    public Age Age { get; private set; }
    public IReadOnlyCollection<Email> Emails => _emails;
    public IReadOnlyCollection<PhoneNumber> Numbers => _numbers;
    public IReadOnlyCollection<ImportantDate> ImportantDates => _dates;

    private Contact(
        Guid id,
        FirstName firstName,
        LastName lastName,
        Sex sex,
        Age age)
        : this(id, sex)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    private Contact(
        Guid id,
        Sex sex) : base(id)
    {
        FirstName = null!;
        LastName = null!;
        Sex = sex;
        Age = null!;
    }

    public bool HasEmail(EmailAddress address) =>
        _emails.Any(e => e.Address == address);
    public bool HasPhone(PhoneNumber number) =>
        _numbers.Any(n => n.Value == number.Value);
    public bool HasImportantDates(ImportantDate importantDate) =>
        _dates.Any(d => d == importantDate);
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

        if (email == null)
        {
            return Result.Failure<Unit>("Nie znaleziono takiego adresu e-mail w tym kontakcie.");
        }
        _emails.Remove(email);
        return Result.Success(Unit.Instance);
    }
    public Result<Unit> AddPhone(PhoneNumber number)
    {
        if (HasPhone(number))
        {
            return Result.Failure<Unit>("Email już istnieje w kontakcie");
        }

        
        _numbers.Add(number);
        return Result.Success(Unit.Instance);
    }
    public Result<Unit> RemovePhone(PhoneNumber number)
    {
        var num = _numbers.FirstOrDefault(e => e.Value == number.Value);

        if (num == null)
        {
            return Result.Failure<Unit>("Nie znaleziono takiego numeru telefonu w tym kontakcie.");
        }
        _numbers.Remove(num);
        return Result.Success(Unit.Instance);
    }
    public Result<Unit> AddImportantDate(ImportantDate importantDate)
    {
        if (HasImportantDates(importantDate))
        {
            return Result.Failure<Unit>("Data już istnieje w kontakcie");
        }

        
        _dates.Add(importantDate);
        return Result.Success(Unit.Instance);
    }
    public Result<Unit> RemoveImportantDate(ImportantDate importantDate)
    {
        var date = _dates.FirstOrDefault(d => d == importantDate);

        if (date == null)
        {
            return Result.Failure<Unit>("Nie znaleziono daty w tym kontakcie.");
        }
        _dates.Remove(date);
        return Result.Success(Unit.Instance);
    }
    public static Result<Contact> Create(
        FirstName firstName,
        LastName lastName,
        Sex sex,
        Age age)
    {
        return Result.Success(new Contact(Guid.NewGuid(), firstName, lastName, sex, age));
    }
    public Result<Unit> UpdateDetails(FirstName firstName, LastName lastName, Sex sex, Age age){
        FirstName = firstName;
        LastName = lastName;
        Sex = sex;
        Age = age;
        return Result.Success(Unit.Instance);
    }
}