namespace Contacts.Application.UseCases.DTOs
{
    public class RemoveImportantDateFromContactDTO
    {
        public int ContactId { get; set; }
        public int ImportantDateId { get; set; }
    }
}