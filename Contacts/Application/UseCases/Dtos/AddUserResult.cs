using Application.Domain;
namespace Application.UseCases.Dtos;
public class AddUserResult(Guid id, string name)
{
    public Guid Id { get; } = id;
    public string UserName { get; } = name;
    internal static AddUserResult FromDomain(User user) =>
        new (user.Id, user.UserName);
}