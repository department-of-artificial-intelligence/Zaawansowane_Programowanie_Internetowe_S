using System;
using System.Collections.Generic;
using System.Linq;

namespace Contacts.Application.Domain;

public class Contact
{
    public int Id { get; private set; }
    public FirstName FirstName { get; private set; } = null!;
    public LastName LastName { get; private set; } = null!;
    public Sex Sex { get; private set; }
    public Age Age { get; private set; } = null!;

    // POPRAWKA: Kolekcja musi być zainicjowana, aby uniknąć błędów
    public ICollection<Email> Emails { get; private set; } = new List<Email>();

    // Konstruktor dla EF Core
    private Contact() { } 

    // POPRAWKA: Publiczny konstruktor do tworzenia NOWEGO kontaktu
    // Nie przyjmuje 'Id' ani 'Emails'
    public Contact(FirstName firstName, LastName lastName, Sex sex, Age age)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Sex = sex;
        Age = age ?? throw new ArgumentNullException(nameof(age));
    }

    // NOWA METODA: Do edycji kontaktu
    public void UpdateBasicInfo(FirstName firstName, LastName lastName, Sex sex, Age age)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Sex = sex;
        Age = age ?? throw new ArgumentNullException(nameof(age));
    }

    // Metody do zarządzania kolekcją
    public bool HasEmail(Email email)
    {
        // Porównujemy obiekty wartości (VO)
        return Emails.Any(e => e.Address == email.Address);
    }
    
    public void AddEmail(Email email)
    {
        if (!HasEmail(email))
        {
            Emails.Add(email);
        }
    }
}