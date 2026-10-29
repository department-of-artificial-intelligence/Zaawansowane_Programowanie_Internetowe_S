namespace WebAPI.Inputs
{
    public record UpdateUserProfileRequest(
     string? FirstName,
     string? LastName,
     DateOnly? DateOfBirth,
     string? Address
 );
}
