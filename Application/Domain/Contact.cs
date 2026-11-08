using Application.Utilities;

namespace Application.Domain;

public class Contact : Entity
{
    private readonly List<Email> _emails = [];

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Sex Sex { get; private set; }
    public Age Age { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public IReadOnlyCollection<Email> Emails => _emails;

    private Contact(
        Guid id,
        string firstName,
        string lastName,
        Sex sex,
        Age age,
        PhoneNumber phoneNumber)
        : this(id, firstName, lastName, sex)
    {
        Age = age;
        PhoneNumber = phoneNumber;
    }

    private Contact(
        Guid id,
        string firstName,
        string lastName,
        Sex sex) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Sex = sex;
        Age = null!;
        PhoneNumber = null!;
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
            return Result.Failure<Unit>("Kontakt nie posiada podanego adresu e-mail.");

        _emails.Remove(email);

        return Result.Success(Unit.Instance);
    }

    public Result<Unit> AddPhoneNumber(PhoneNumber phoneNumber)
    {
        if(PhoneNumber != null) return Result.Failure<Unit>("Kontakt posiada numer telefonu.");

        return PhoneNumber
            .Create(phoneNumber, categoryId)
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemovePhoneNumber()
    {
        if(PhoneNumber == null) return Result.Failure<Unit>("Kontakt nie posiada numeru telefonu.");

        PhoneNumber = null;

        return Result.Success(Unit.Instance);
    }

    public static Result<Contact> Create(
        string firstName,
        string lastName,
        Sex sex,
        Age age)
    {
        return Result.Combine(
            Validators.NotEmpty(firstName, "Imię jest wymagane."),
            Validators.NotEmpty(lastName, "Nazwisko jest wymagane.")
        ).Map(_ => new Contact(Guid.NewGuid(), firstName, lastName, sex, age));
    }

    public Result<Unit> Edit(
        string firstName,
        string lastName,
        Sex sex,
        int age)
        {
            var firstNameResult = FirstName.Create(firstName);
            var lastNameResult = LastName.Create(lastName);
            var ageResult = Age.Create(age);

            return Result.Combine(
                firstNameResult,
                lastNameResult,
                ageResult
            ).Tap(_ =>
            {
                FirstName = firstNameResult.Value;
                LastName = lastNameResult.Value;
                Sex = sex;
                Age = ageResult.Value;
            }).Map(_ => Unit.Instance);
        }
}