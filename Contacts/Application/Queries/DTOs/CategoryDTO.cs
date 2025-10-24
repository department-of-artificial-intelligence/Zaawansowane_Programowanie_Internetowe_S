namespace Contacts.Application.Queries.DTOs
{
    public sealed class CategoryDTO
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        
        public CategoryDTO() {}
        public CategoryDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}