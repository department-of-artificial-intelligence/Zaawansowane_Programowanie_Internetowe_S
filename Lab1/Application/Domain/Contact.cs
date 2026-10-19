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
        string firstName,
        string lastName,
        Sex sex,
        Age age)
    {
        return Result.Combine(
            FirstName.Create(firstName),
            LastName.Create(lastName)
        ).Map(_ => new Contact(
            Guid.NewGuid(),
            FirstName.Create(firstName).Value,
            LastName.Create(lastName).Value,
            sex,
            age));
    }
}