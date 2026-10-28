namespace Application.OutputPorts;

// Zadanie 7 - wyliczanie skrótu hasła jest kwestią infrastrukturalną,
// dlatego w bibliotece Application definiujemy tylko interfejs.
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string passwordHash, string password);
}
