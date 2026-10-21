using Application.Domain;

namespace WebAPI;

public record RenameCategoryRequest(string Name);

public record UpdateContactRequest(string FirstName, string LastName, Sex Sex, int Age);

public record AddEmailRequest(string Address, Guid CategoryId);

public record AddPhoneRequest(string Number, PhoneType Type);

public record AddImportantDateRequest(DateOnly Date, string Label);