using Application.Domain;
namespace WebAPI;

// Ciała żądań dla akcji, w których identyfikator jest w ścieżce (np. PUT /api/categories/{id}).
// Dzięki temu Id nie jest podawane dwa razy (w ścieżce i w JSON-ie) i nie może się różnić.

public record RenameCategoryRequest(string Name);

public record UpdateContactRequest(string FirstName, string LastName, Sex Sex, int Age);

public record AddEmailRequest(string Address, Guid CategoryId);

public record AddPhoneRequest(string Number, PhoneType Type);

public record ImportantDateRequest(string Label, DateOnly Date);

public record CreatedIdResponse(Guid Id);
