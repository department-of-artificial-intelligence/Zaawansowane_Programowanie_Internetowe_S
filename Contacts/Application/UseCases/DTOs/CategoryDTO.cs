using Contacts.Application.Domain; 

namespace Contacts.Application.UseCases.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public static explicit operator CategoryDTO(Category category)
        {
            return new CategoryDTO
            {
                Id = category.Id, 
                Name = category.Name
            };
        }
    }
}