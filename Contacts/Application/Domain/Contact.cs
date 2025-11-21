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

    public ICollection<Email> Emails { get; private set; } = new List<Email>();
    public ICollection<Phone> Phones { get; private set; } = new List<Phone>();
    
    // --- NOWA KOLEKCJA DLA WAŻNYCH DAT ---
    public ICollection<ImportantDate> ImportantDates { get; private set; } = new List<ImportantDate>();

    // Konstruktor dla EF Core
    private Contact() { } 

    // Publiczny konstruktor (bez zmian)
    public Contact(FirstName firstName, LastName lastName, Sex sex, Age age)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Sex = sex;
        Age = age ?? throw new ArgumentNullException(nameof(age));
    }

    // Metoda do edycji kontaktu (bez zmian)
    public void UpdateBasicInfo(FirstName firstName, LastName lastName, Sex sex, Age age)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Sex = sex;
        Age = age ?? throw new ArgumentNullException(nameof(age));
    }

    // --- Metody do zarządzania Email (bez zmian) ---
    public bool HasEmail(Email email) => Emails.Any(e => e.Address == email.Address);
    public void AddEmail(Email email) { if (!HasEmail(email)) Emails.Add(email); }
    public Email? FindEmailById(int emailId) => Emails.SingleOrDefault(e => e.Id == emailId);
    public void RemoveEmail(Email email) { if (email != null) Emails.Remove(email); }
    
    // --- Metody do zarządzania Telefonami (bez zmian) ---
    public bool HasPhone(Phone phone) => Phones.Any(p => p.Number == phone.Number);
    public void AddPhone(Phone phone) { if (!HasPhone(phone)) Phones.Add(phone); }
    public Phone? FindPhoneById(int phoneId) => Phones.SingleOrDefault(p => p.Id == phoneId);
    public void RemovePhone(Phone phone) { if (phone != null) Phones.Remove(phone); }

    // --- NOWE METODY DO ZARZĄDZANIA DATAMI ---
    public void AddImportantDate(ImportantDate date)
    {
        // Można dodać logikę sprawdzającą duplikaty, jeśli potrzeba
        ImportantDates.Add(date);
    }

    public ImportantDate? FindImportantDateById(int dateId)
    {
        return ImportantDates.SingleOrDefault(d => d.Id == dateId);
    }

    public void RemoveImportantDate(ImportantDate date)
    {
        if (date != null)
        {
            ImportantDates.Remove(date);
        }
    }
}