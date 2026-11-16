using System;
using Application.Domain;

namespace Application.UseCases.Dtos
{
    public class UpdateCategoryResult(Guid id, string name)
    {
        public Guid Id { get; } = id;
        public string Name { get; } = name;

        internal static UpdateCategoryResult FromDomain(Category category) => new(category.Id, category.Name);
    }
}