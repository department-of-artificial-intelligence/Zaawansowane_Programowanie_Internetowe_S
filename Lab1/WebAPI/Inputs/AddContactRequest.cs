using Application.Domain;

namespace WebAPI.Inputs;

public class AddContactRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Sex Sex { get; set; }
    public int Age { get; set; }
}
