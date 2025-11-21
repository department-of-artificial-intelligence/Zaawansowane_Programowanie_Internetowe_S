using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.Application.Domain;

public class Category(string name, int id)
{
    // Dodaj walidację również do konstruktora
    public Category(string name) : this(ValidateName(name), 0) { }
    
    // Prywatny konstruktor dla EF Core (jeśli używasz primary constructor)

    public int Id { get; private set; } = id;
    public string Name { get; private set; } = ValidateName(name);

    // --- NOWA METODA ---
    // Metoda do aktualizacji nazwy
    public void UpdateName(string newName)
    {
        Name = ValidateName(newName);
    }

    // Prywatna metoda walidacyjna
    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Nazwa kategorii nie może być pusta.", nameof(name));
        }
        // Możesz dodać więcej reguł, np. długość
        return name;
    }
}