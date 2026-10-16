using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public class Contact : Entity
    {
        private readonly List<Email> _emails = [];
        private readonly List<Phone> _phones = [];
        private readonly List<ImportantDate> _importantDates = [];

        public FirstName FirstName { get; private set; }
        public LastName LastName { get; private set; }
        public Sex Sex { get; private set; }
        public Age Age { get; private set; }

        public IReadOnlyCollection<Phone> Phones => _phones;
        public IReadOnlyCollection<Email> Emails => _emails;
        public IReadOnlyCollection<ImportantDate> ImportantDates => _importantDates;

        private Contact(
           Guid id,
           FirstName firstName,
           LastName lastName,
           Sex sex,
           Age age) : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Sex = sex;
            Age = age;
        }

        private Contact(Guid id) : base(id)
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

        public static Result<Contact> Create(
            FirstName firstName,
            LastName lastName,
            Sex sex,
            Age age)
        {
            return Result.Success(new Contact(Guid.NewGuid(), firstName, lastName, sex, age));
        }


        //public static Result<Contact> Create(
        //   string firstName,
        //  string lastName,
        //  Sex sex,
        //  Age age)
        //{
        // return Result.Combine(
        //  Validators.NotEmpty(firstName, "Imię jest wymagane."),
        //  Validators.NotEmpty(lastName, "Nazwisko jest wymagane.")
        //  ).Map(_ => new Contact(Guid.NewGuid(), firstName, lastName, sex, age));

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
            var email = _emails.FirstOrDefault(e => e.Address == address);
            if (email is null)
            {
                return Result.Failure<Unit>("Podany adres e-mail nie istnieje w tym kontakcie.");
            }

            _emails.Remove(email);
            return Result.Success(Unit.Instance);
        }

        public bool HasPhone(PhoneNumber number) =>
    _phones.Any(p => p.Number == number);

        public Result<Unit> AddPhone(PhoneNumber number, Guid categoryId)
        {
            if (HasPhone(number))
            {
                return Result.Failure<Unit>("Numer telefonu już istnieje w tym kontakcie.");
            }

            return Phone
                .Create(number, categoryId)
                .Tap(_phones.Add)
                .Map(_ => Unit.Instance);
        }

        public Result<Unit> RemovePhone(PhoneNumber number)
        {
            var phone = _phones.FirstOrDefault(p => p.Number == number);
            if (phone is null)
            {
                return Result.Failure<Unit>("Podany numer telefonu nie istnieje w tym kontakcie.");
            }

            _phones.Remove(phone);
            return Result.Success(Unit.Instance);
        }

        public bool HasImportantDate(DateDescription description) =>
    _importantDates.Any(d => d.Description == description);

        public Result<Unit> AddImportantDate(DateOnly date, DateDescription description)
        {
            if (HasImportantDate(description))
            {
                return Result.Failure<Unit>($"Ważna data o opisie '{description.Value}' już istnieje w tym kontakcie.");
            }

            return ImportantDate
                .Create(date, description)
                .Tap(_importantDates.Add)
                .Map(_ => Unit.Instance);
        }

        public Result<Unit> RemoveImportantDate(DateDescription description)
        {
            var importantDate = _importantDates.FirstOrDefault(d => d.Description == description);
            if (importantDate is null)
            {
                return Result.Failure<Unit>($"Ważna data o opisie '{description.Value}' nie istnieje w tym kontakcie.");
            }

            _importantDates.Remove(importantDate);
            return Result.Success(Unit.Instance);
        }

    }
}
