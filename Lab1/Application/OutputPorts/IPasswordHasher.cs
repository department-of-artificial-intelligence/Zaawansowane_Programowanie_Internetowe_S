namespace Application.OutputPorts;

// Zadanie 7: haszowanie to kwestia infrastruktury, więc domena zna tylko interfejs
// (tak samo jak ITokenGenerator). Implementacja: WebAPI/Helpers/PasswordHasher.cs
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string passwordHash, string password);
}
