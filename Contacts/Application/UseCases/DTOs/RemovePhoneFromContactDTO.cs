namespace Contacts.Application.UseCases.DTOs
{
    public class RemovePhoneFromContactDTO
    {
        public int ContactId { get; set; }
        public int PhoneId { get; set; } // ID numeru telefonu, nie kontaktu
    }
}