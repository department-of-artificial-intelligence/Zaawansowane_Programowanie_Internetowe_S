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
        : this(id, sex)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    // Konstruktor dla Entity Framework
    private Contact(Guid id, Sex sex) : base(id)
    {
        Sex = sex;
        FirstName = null!;
        LastName = null!;
        Age = null!;
    }

    // ---------- Dane podstawowe ----------

    public void UpdateDetails(
        FirstName firstName,
        LastName lastName,
        Sex sex,
        Age age)
    {
        FirstName = firstName;
        LastName = lastName;
        Sex = sex;
        Age = age;
    }

    // ---------- E-maile ----------

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
            return Result.Failure<Unit>("Kontakt nie posiada takiego adresu e-mail.");
        }

        _emails.Remove(email);
        return Result.Success(Unit.Instance);
    }

    // ---------- Telefony ----------

    public bool HasPhone(PhoneNumber number) =>
        _phones.Any(p => p.Number == number);

    public Result<Unit> AddPhone(PhoneNumber number, PhoneType type)
    {
        if (HasPhone(number))
        {
            return Result.Failure<Unit>("Numer telefonu już istnieje w kontakcie.");
        }

        return Phone
            .Create(number, type)
            .Tap(_phones.Add)
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemovePhone(PhoneNumber number)
    {
        var phone = _phones.FirstOrDefault(p => p.Number == number);

        if (phone is null)
        {
            return Result.Failure<Unit>("Kontakt nie posiada takiego numeru telefonu.");
        }

        _phones.Remove(phone);
        return Result.Success(Unit.Instance);
    }

    // ---------- Ważne daty ----------

    public bool HasImportantDate(DateLabel label) =>
        _importantDates.Any(d =>
            string.Equals(d.Label.Value, label.Value, StringComparison.OrdinalIgnoreCase));

    public Result<Unit> AddImportantDate(DateOnly date, DateLabel label)
    {
        if (HasImportantDate(label))
        {
            return Result.Failure<Unit>(
                $"Kontakt ma już datę o nazwie '{label.Value}'.");
        }

        return ImportantDate
            .Create(date, label)
            .Tap(_importantDates.Add)
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemoveImportantDate(DateLabel label)
    {
        var importantDate = _importantDates.FirstOrDefault(d =>
            string.Equals(d.Label.Value, label.Value, StringComparison.OrdinalIgnoreCase));

        if (importantDate is null)
        {
            return Result.Failure<Unit>(
                $"Kontakt nie posiada daty o nazwie '{label.Value}'.");
        }

        _importantDates.Remove(importantDate);
        return Result.Success(Unit.Instance);
    }

    // ---------- Tworzenie ----------

    public static Result<Contact> Create(
        FirstName firstName,
        LastName lastName,
        Sex sex,
        Age age) =>
        Result.Success(
            new Contact(Guid.NewGuid(), firstName, lastName, sex, age));
}