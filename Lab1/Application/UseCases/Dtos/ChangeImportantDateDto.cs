namespace Application.UseCases.Dtos;
public record ChangeImportantDateDto(Guid ContactId, Guid DateId, string Label, DateOnly Date);