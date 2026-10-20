namespace Application.UseCases.Dtos;
public record AddImportantDateDto(Guid ContactId, string Label, DateOnly Date);