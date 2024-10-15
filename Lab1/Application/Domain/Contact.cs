using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Contacts.Application.Domain;

public class Contact
{
    public int Id { get; private set;}
    public string FirstName { get; private set;} = string.Empty;
    public string LastName {get; private set;} = string.Empty;
    public Sex Sex {get; private set;}
    public ICollection<Email> Emails {get; private set;} = null!;
    public Age Age {get; private set;} = null!;

    private Contact() {}     
    public Contact(int id, string firstName, string lastName,
                Sex sex, ICollection<Email> emails, Age age){
        Id = id;
        FirstName = 
            string.IsNullOrWhiteSpace(firstName) 
            ? throw new ArgumentException(nameof(FirstName), nameof(firstName)) : firstName;
        LastName =
            string.IsNullOrWhiteSpace(lastName) 
            ? throw new ArgumentException(nameof(LastName), nameof(lastName)) : lastName;
        Sex = sex;
        Emails = emails ?? throw new ArgumentNullException(nameof(emails));
        Age = age ?? throw new ArgumentNullException(nameof(age));
        }   
}
