using Application.Domain;

namespace WebAPI.Inputs
{
    public record AddContactRequest(string FirstName, string LastName, Sex Sex, int Age);
}
