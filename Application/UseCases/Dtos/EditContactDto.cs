using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public record EditContactDto(
    Guid contactId;
    string firstName;
    string lastName;
    Sex sex;
    int age
)
