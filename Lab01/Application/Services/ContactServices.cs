// using Contacts.Application.Domain;
// using Contacts.Application.Repositories;
// using Contacts.Application.UseCases;
// using Contacts.Application.UseCases.DTOs;

// namespace Contacts.Application.Services;

// public class ContactServices : IContactUseCases
// {
//     private readonly IContactRepository _contactRepository;
//     private readonly ICategoryServices _categoryRepository;
//     private readonly IUnitOfWork _unitOfWork;

//     public ContactServices(IContactRepository contactRepository, ICategoryServices categoryRepository, IUnitOfWork unitOfWork)
//     {
//         _contactRepository = contactRepository;
//         _categoryRepository = categoryRepository;
//         _unitOfWork = unitOfWork;
//     }

//     public void AddContact(AddContactDTO dto)
//     {
//         try
//         {
//             var category = _categoryRepository.GetCategoryById(dto.CategoryId);
//             if (category == null)
//                 throw new ServiceException($"Kategoria o ID {dto.CategoryId} nie istnieje");

//             var firstName = new FirstName(dto.FirstName);
//             var lastName = new LastName(dto.LastName);
//             var sex = (Sex)dto.Sex;
//             var age = new Age(dto.Age);

//             var emails = new List<Email>();
//             foreach (var emailStr in dto.Emails)
//             {
//                 var emailAddress = new EmailAddress(emailStr);
//                 //emails.Add(new Email(emailAddress));
//             }

//             var contact = new Contact(0, firstName, lastName, sex, emails, age);
//             category.AddContact(contact);

//             _contactRepository.Add(contact);
//             _unitOfWork.SaveChanges();
//         }
//         catch (ArgumentException ex)
//         {
//             throw new ServiceException($"Błąd walidacji: {ex.Message}", ex);
//         }
//     }

//     public void UpdateContact(UpdateContactDTO dto)
//     {
//         try
//         {
//             var contact = _contactRepository.GetById(dto.Id);
//             if (contact == null)
//                 throw new ServiceException($"Kontakt o ID {dto.Id} nie istnieje");

//             var firstName = new FirstName(dto.FirstName);
//             var lastName = new LastName(dto.LastName);
//             var sex = (Sex)dto.Sex;
//             var age = new Age(dto.Age);

//             var emails = new List<Email>();
//             foreach (var emailStr in dto.Emails)
//             {
//                 var emailAddress = new EmailAddress(emailStr);
//                 //emails.Add(new Email(emailAddress));
//             }

//             var updatedContact = new Contact(dto.Id, firstName, lastName, sex, emails, age);
//             _contactRepository.Update(updatedContact);
//             _unitOfWork.SaveChanges();
//         }
//         catch (ArgumentException ex)
//         {
//             throw new ServiceException($"Błąd walidacji: {ex.Message}", ex);
//         }
//     }

//     public void RemoveContact(int id)
//     {
//         var contact = _contactRepository.GetById(id);
//         if (contact == null)
//             throw new ServiceException($"Kontakt o ID {id} nie istnieje");

//         _contactRepository.Remove(contact);
//         _unitOfWork.SaveChanges();
//     }

//     public ContactDTO? GetContact(int id)
//     {
//         var contact = _contactRepository.GetById(id);
//         if (contact == null)
//             return null;

//         return new ContactDTO
//         {
//             Id = contact.Id,
//             FirstName = contact.FirstName.Value,
//             LastName = contact.LastName.Value,
//             Sex = contact.Sex.ToString(),
//             Age = contact.Age.Value,
//             Emails = contact.Emails.Select(e => e.Address.Value).ToList()
//         };
//     }

//     public IEnumerable<ContactDTO> GetAllContacts()
//     {
//         return _contactRepository.GetAll().Select(contact => new ContactDTO
//         {
//             Id = contact.Id,
//             FirstName = contact.FirstName.Value,
//             LastName = contact.LastName.Value,
//             Sex = contact.Sex.ToString(),
//             Age = contact.Age.Value,
//             Emails = contact.Emails.Select(e => e.Address.Value).ToList()
//         });
//     }
// }