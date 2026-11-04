namespace WebAPI.Inputs;

public class UpdateUserProfileRequest
{

    public string FirstName {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty;
    public DateOnly BirthDate {get; set;}
    public string Address {get; set;} = string.Empty;
}
