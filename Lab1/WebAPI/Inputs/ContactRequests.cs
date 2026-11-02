using Application.Domain;
namespace WebAPI.Inputs;

// Zadanie 8: właściciel kontaktu z tokenu, więc klient podaje tylko dane kontaktu
public record AddContactRequest(string FirstName, string LastName, Sex Sex, int Age);
