using Contacts.Application.Domain;
namespace Contacts.Application.UseCases.DTOs;

public class AddUserResult
{
    public int Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public static implicit operator AddUserResult(User user) => new AddUserResult { Id = user.Id, UserName = user.UserName };
}