using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.Application.Domain;

public class Category(string name, int id)
{
    public Category(string name) : this(ValidateName(name), 0) { }

    public int Id { get; private set; } = id;
    public string Name { get; private set; } = ValidateName(name);

    public void UpdateName(string newName)
    {
        Name = ValidateName(newName);
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Nazwa kategorii nie może być pusta.", nameof(name));
        }
        return name;
    }
}