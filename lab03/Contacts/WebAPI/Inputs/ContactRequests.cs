using Application.Domain;

namespace WebAPI.Inputs;

public record AddContactRequest(string FirstName, string LastName, Sex Sex, int Age);
public record UpdateContactRequest(string FirstName, string LastName, Sex Sex, int Age);
public record AddEmailRequest(string Address, Guid CategoryId);
public record PhoneRequest(string Number);
public record ImportantDateRequest(DateOnly Date, string Description);
