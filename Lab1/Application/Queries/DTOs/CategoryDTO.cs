using Contacts.Application.Domain;

namespace Contacts.Application.Queries.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public static explicit operator CategoryDTO(Category v)
        {
            throw new NotImplementedException();
        }
    }
}
