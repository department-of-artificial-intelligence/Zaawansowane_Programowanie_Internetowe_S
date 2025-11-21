namespace Contacts.Application.UseCases.DTOs
{
    public class AddPhoneToContactDTO
    {
        public int ContactId { get; set; }
        public string Number { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}