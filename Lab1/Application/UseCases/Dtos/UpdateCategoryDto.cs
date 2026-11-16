using System;

namespace Application.UseCases.Dtos
{
    public class UpdateCategoryDto(Guid id, string name)
    {
        public Guid Id { get; set; } = id;
        public string Name { get; set; } = name;
    }
}