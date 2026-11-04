using Application.Utilities;
namespace Application.Domain;
public class Contact : Entity
{
public Guid UserId { get; private set; }
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
Age age,
Guid userId)
: this(id, firstName, lastName, sex)
{
Age = age;
UserId = userId;
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
public Result<Unit> Edit(
FirstName firstName, LastName lastName, Sex sex, Age age)
{
FirstName = firstName;
LastName = lastName;
Sex = sex;
Age = age;
return Result.Success(Unit.Instance);
}
public Result<Unit> RemoveEmail(EmailAddress address)
{
var email = _emails.SingleOrDefault(item => item.Address == address);
return email is null
? Result.Failure<Unit>("Email nie istnieje w kontakcie")
: Remove(_emails, email);
}
public Result<Unit> AddPhoneNumber(PhoneNumber phoneNumber) =>
_phoneNumbers.Any(number => number == phoneNumber)
? Result.Failure<Unit>("Numer telefonu już istnieje w kontakcie")
: Add(_phoneNumbers, phoneNumber);
public Result<Unit> RemovePhoneNumber(PhoneNumber phoneNumber)
{
var existing = _phoneNumbers.SingleOrDefault(number => number == phoneNumber);
return existing is null
? Result.Failure<Unit>("Numer telefonu nie istnieje w kontakcie")
: Remove(_phoneNumbers, existing);
}
public Result<Unit> AddImportantDate(ImportantDate importantDate) =>
_importantDates.Any(date => date == importantDate)
? Result.Failure<Unit>("Ważna data już istnieje w kontakcie")
: Add(_importantDates, importantDate);
public Result<Unit> RemoveImportantDate(ImportantDate importantDate)
{
var existing = _importantDates.SingleOrDefault(date => date == importantDate);
return existing is null
? Result.Failure<Unit>("Ważna data nie istnieje w kontakcie")
: Remove(_importantDates, existing);
}
private static Result<Unit> Add<T>(ICollection<T> collection, T item)
{
collection.Add(item);
return Result.Success(Unit.Instance);
}
private static Result<Unit> Remove<T>(ICollection<T> collection, T item)
{
collection.Remove(item);
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
public static Result<Contact> Create(
string firstName,
string lastName,
Sex sex,
Age age,
Guid userId = default)
{
return FirstName.Create(firstName)
.Bind(first => LastName.Create(lastName)
.Map(last => new Contact(Guid.NewGuid(), first, last, sex, age, userId)));
}
}
