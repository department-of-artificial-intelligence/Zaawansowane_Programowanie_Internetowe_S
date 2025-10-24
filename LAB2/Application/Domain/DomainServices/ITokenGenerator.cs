namespace Contacts.Application.DomainServices;

public interface ITokenGenerator
{
    string GenerateToken(int userId, string userName);
}