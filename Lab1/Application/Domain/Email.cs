using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.Application.Domain;

public class Email
{
    public int Id{get;private set;}
    public EmailAdress Adress{get;private set;} = null;
    public Contact Contact{get;private set;}=null;
    public int ContactId{get;private set;}
    public int CategoryId{get;private set;}
    public Category Category{get;private set;} =null;
    
    private Email()
    {
           
    }

    public Email(int id, Contact contact, Category category, EmailAdress email)
    {
        Id = id;
        CategoryId = category.Id;
        ContactId = contact.Id;
        Contact = contact;
        Category = category;
        Adress = email; 
    }

}
