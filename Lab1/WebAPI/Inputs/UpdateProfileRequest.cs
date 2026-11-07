namespace WebAPI.Inputs;

public record UpdateProfileRequest(string? FirstName, string? LastName, DateOnly? BirthDate, string? Address);