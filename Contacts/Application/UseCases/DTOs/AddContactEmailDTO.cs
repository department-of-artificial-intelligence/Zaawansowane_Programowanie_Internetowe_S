namespace Contacts.Application.UseCases.DTOs
{
    // Ten DTO będzie reprezentował pojedynczy email podczas tworzenia kontaktu
    public class AddContactEmailDTO
    {
        public string Address { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}