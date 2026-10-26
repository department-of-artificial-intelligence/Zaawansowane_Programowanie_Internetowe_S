namespace Application.OutputPorts;
public interface ITokenGenerator
{
    string GenerateToken(Guid userId, string userName);
}