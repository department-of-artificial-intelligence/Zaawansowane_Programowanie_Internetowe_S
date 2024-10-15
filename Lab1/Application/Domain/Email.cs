using System.ComponentModel;

namespace Contacts.Application.Domain;

public class Email{
    public int Id{get; private set;}
    public EmailAddress Address{get; private set;} = null!;
    public Contact Contact {get; private set;} = null!;
    public int ContactId{get; private set;}
    public int CategoryId{get; private set;}
    public Category Category {get; private set;} = null!;
    public Email(){

    }
    public Email (int id, Contact contact, Category category, EmailAddress address, int contactId,){
        Id = id;
        CategoryId = category.Id;
        ContactId = Contact.Id;
        Contact = contact;
        Category = category;
        Address = address;
    }

}