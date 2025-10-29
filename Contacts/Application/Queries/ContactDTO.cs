using Contacts.Application.Domain;

namespace Contacts.Application.Queries.DTOs
{
    public class ContactDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Sex Sex { get; set; }
        public ICollection<Email> Emails { get; set; } = null!;
        public Age Age { get; set; } = null!;

        public static explicit operator ContactDTO(Contact contact)
        {
            return new ContactDTO
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Sex = contact.Sex,
                Emails = contact.Emails,
                Age = contact.Age,
            };
        }
    }
}