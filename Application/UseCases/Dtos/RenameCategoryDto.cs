using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public record RenameCategoryDto(
    Guid CategoryId;
    string Name;
)