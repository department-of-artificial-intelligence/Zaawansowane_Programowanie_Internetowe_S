namespace Contacts.Application.Domain
{
    public class Contact
    {
        public int Id { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public Sex Sex { get; private set; }
        public ICollection<Email> Emails { get; private set; } = new List<Email>();
        public Age Age { get; private set; }

        // Prywatny konstruktor potrzebny do zachowania enkapsulacji
        private Contact() { }

        public Contact(int id, string firstName, string lastName, Sex sex, ICollection<Email> emails, Age age)
        {
            Id = id;

            FirstName = string.IsNullOrWhiteSpace(firstName) 
                ? throw new ArgumentException(nameof(firstName)) 
                : firstName;

            LastName = string.IsNullOrWhiteSpace(lastName) 
                ? throw new ArgumentException(nameof(lastName)) 
                : lastName;

            Sex = sex;
            Emails = emails ?? throw new ArgumentNullException(nameof(emails));
            Age = age ?? throw new ArgumentNullException(nameof(age));
        }

        // Sprawdza, czy kontakt ma podany email
        public bool HasEmail(Email email)
        {
            return Emails.Any(e => e.Address == email.Address);
        }

        // Dodaje nowy email, jeśli jeszcze nie istnieje
        public void AddEmail(Email email)
        {
            if (!HasEmail(email))
            {
                Emails.Add(email);
            }
        }
    }
}
