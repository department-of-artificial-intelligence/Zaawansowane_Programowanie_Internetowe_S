namespace Contacts.Application.UseCases.DTOs
{
    public class AddContactEmailDTO
    {
        public string Address { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}