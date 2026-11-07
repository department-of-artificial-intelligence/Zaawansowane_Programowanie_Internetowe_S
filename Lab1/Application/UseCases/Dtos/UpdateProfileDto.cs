namespace Application.UseCases.Dtos;

public record UpdateProfileDto(Guid UserId,string? FirstName,string? LastName,DateOnly? BirthDate,string? Address);
