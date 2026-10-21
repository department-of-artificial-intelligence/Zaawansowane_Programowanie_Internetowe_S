using Application.Utilities;

namespace Application.Domain;

public class Contact : Entity
{
    private readonly List<Email> _emails = [];
    private readonly List<PhoneNumber> _phoneNumbers = [];
    private readonly List<ImportantDate> _importantDates = [];

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Sex Sex { get; private set; }
    public Age Age { get; private set; }
    public IReadOnlyCollection<Email> Emails => _emails;
    public IReadOnlyCollection<PhoneNumber> PhoneNumbers => _phoneNumbers;
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

    public Result<Unit> Update(
        FirstName firstName,
        LastName lastName,
        Sex sex,
        Age age)
    {
        FirstName = firstName;
        LastName = lastName;
        Sex = sex;
        Age = age;
        return Result.Success(Unit.Instance);
    }

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

    public Result<Unit> RemoveEmail(EmailAddress address) =>
        _emails.RemoveAll(e => e.Address == address) > 0
            ? Result.Success(Unit.Instance)
            : Result.Failure<Unit>("Kontakt nie ma takiego adresu email.");

    public Result<Unit> AddPhoneNumber(PhoneNumber number)
    {
        if (_phoneNumbers.Contains(number))
        {
            return Result.Failure<Unit>("Numer telefonu już istnieje w kontakcie.");
        }

        _phoneNumbers.Add(number);
        return Result.Success(Unit.Instance);
    }

    public Result<Unit> RemovePhoneNumber(PhoneNumber number) =>
        _phoneNumbers.Remove(number)
            ? Result.Success(Unit.Instance)
            : Result.Failure<Unit>("Kontakt nie ma takiego numeru telefonu.");

    public Result<Unit> AddImportantDate(ImportantDate date)
    {
        if (_importantDates.Contains(date))
        {
            return Result.Failure<Unit>("Ta ważna data już istnieje w kontakcie.");
        }

        _importantDates.Add(date);
        return Result.Success(Unit.Instance);
    }

    public Result<Unit> RemoveImportantDate(ImportantDate date) =>
        _importantDates.Remove(date)
            ? Result.Success(Unit.Instance)
            : Result.Failure<Unit>("Kontakt nie ma takiej ważnej daty.");

    public static Result<Contact> Create(
        FirstName firstName,
        LastName lastName,
        Sex sex,
        Age age) =>
        Result.Success(new Contact(Guid.NewGuid(), firstName, lastName, sex, age));
}
