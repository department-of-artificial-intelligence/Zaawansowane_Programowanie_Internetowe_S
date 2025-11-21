using Contacts.Application.Domain;
namespace Contacts.Application.UseCases.DTOs
{
    public class UpdateContactDTO
    {
        public int Id { get; set; } // ID kontaktu, który edytujemy
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Sex Sex { get; set; }
        public int Age { get; set; }
    }
}