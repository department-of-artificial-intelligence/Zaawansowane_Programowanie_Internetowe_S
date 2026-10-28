using Application.Utilities;

namespace Application.Domain;

public class Contact : Entity
{
    private readonly List<Email> _emails = [];
    private readonly List<PhoneNumber> _phones = [];
    private readonly List<ImportantDate> _importantDates = [];

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Sex Sex { get; private set; }
    public Age Age { get; private set; }

    // Zadanie 8 - każdy kontakt należy do użytkownika.
    public Guid UserId { get; private set; }
    public IReadOnlyCollection<Email> Emails => _emails;
    public IReadOnlyCollection<PhoneNumber> Phones => _phones;
    public IReadOnlyCollection<ImportantDate> ImportantDates => _importantDates;

    private Contact(
        Guid id,
        FirstName firstName,
        LastName lastName,
        Sex sex,
        Age age,
        Guid userId)
        : this(id, sex, userId)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    // Konstruktor wykorzystywany przez Entity Framework.
    private Contact(
        Guid id,
        Sex sex,
        Guid userId) : base(id)
    {
        Sex = sex;
        UserId = userId;
        FirstName = null!;
        LastName = null!;
        Age = null!;
    }

    // Zadanie 5 - edycja danych kontaktu.
    public Result<Unit> Update(
        string firstName,
        string lastName,
        Sex sex,
        Age age) =>
        CreateNames(firstName, lastName)
            .Map(names =>
            {
                FirstName = names.FirstName;
                LastName = names.LastName;
                Sex = sex;
                Age = age;
                return Unit.Instance;
            });

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

    // Zadanie 6 - usuwanie adresu email.
    public Result<Unit> RemoveEmail(EmailAddress address) =>
        _emails.RemoveAll(e => e.Address == address) > 0
            ? Result.Success(Unit.Instance)
            : Result.Failure<Unit>($"Kontakt nie posiada adresu email {address.Value}");

    // Zadanie 7 - numery telefonów.
    public Result<Unit> AddPhone(PhoneNumber number)
    {
        if (_phones.Contains(number))
        {
            return Result.Failure<Unit>("Numer telefonu już istnieje w kontakcie");
        }

        _phones.Add(number);
        return Result.Success(Unit.Instance);
    }

    public Result<Unit> RemovePhone(PhoneNumber number) =>
        _phones.RemoveAll(p => p == number) > 0
            ? Result.Success(Unit.Instance)
            : Result.Failure<Unit>($"Kontakt nie posiada numeru telefonu {number.Value}");

    // Zadanie 8 - ważne daty. Niezmiennik: opis daty jest unikalny w obrębie
    // kontaktu (np. kontakt może mieć tylko jedne "Urodziny").
    public Result<Unit> AddImportantDate(ImportantDate importantDate)
    {
        if (_importantDates.Any(d => SameDescription(d, importantDate.Description)))
        {
            return Result.Failure<Unit>(
                $"Kontakt posiada już datę o opisie '{importantDate.Description}'");
        }

        _importantDates.Add(importantDate);
        return Result.Success(Unit.Instance);
    }

    public Result<Unit> RemoveImportantDate(string description) =>
        _importantDates.RemoveAll(d => SameDescription(d, description)) > 0
            ? Result.Success(Unit.Instance)
            : Result.Failure<Unit>($"Kontakt nie posiada daty o opisie '{description}'");

    private static bool SameDescription(ImportantDate date, string description) =>
        string.Equals(date.Description, description.Trim(), StringComparison.OrdinalIgnoreCase);

    public static Result<Contact> Create(
        string firstName,
        string lastName,
        Sex sex,
        Age age,
        Guid userId) =>
        CreateNames(firstName, lastName)
            .Map(names => new Contact(Guid.NewGuid(), names.FirstName, names.LastName, sex, age, userId));

    // Zwraca błędy obu pól naraz (jak Result.Combine), a nie tylko pierwszy.
    private static Result<(FirstName FirstName, LastName LastName)> CreateNames(
        string firstName, string lastName)
    {
        var first = FirstName.Create(firstName);
        var last = LastName.Create(lastName);

        return Result.Combine(
                first.Map(_ => Unit.Instance),
                last.Map(_ => Unit.Instance))
            .Bind(_ => first.Bind(f => last.Map(l => (f, l))));
    }
}
