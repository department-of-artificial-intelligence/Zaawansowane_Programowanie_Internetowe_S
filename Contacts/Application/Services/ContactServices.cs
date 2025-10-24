using Contacts.Application.Domain;
using Contacts.Application.Repositories;
using Contacts.Application.UseCases.DTOs;
using System.Threading.Tasks;

namespace Contacts.Application.Services
{
    public class ContactServices : IContactUseCases
    {
        private readonly IContactRepository _contactRepository;
        private readonly ICategoryRepository _categoryRepository; 
        private readonly IUnitOfWork _unitOfWork;

        public ContactServices(
            IContactRepository contactRepository,
            ICategoryRepository categoryRepository, 
            IUnitOfWork unitOfWork)
        {
            _contactRepository = contactRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task AddContactAsync(AddContactDTO dto)
        {
            var firstName = new FirstName(dto.FirstName);
            var lastName = new LastName(dto.LastName);
            var age = new Age(dto.Age);

            var contact = new Contact(firstName, lastName, dto.Sex, age);

            foreach (var emailDto in dto.Emails)
            {
                var category = _categoryRepository.GetCategory(emailDto.CategoryId);
                if (category == null)
                {
                    throw new ServiceException($"Kategoria o ID: {emailDto.CategoryId} nie istnieje.");
                }
                
                var emailAddress = new EmailAddress(emailDto.Address);
                var email = new Email(emailAddress, category.Id);

                contact.AddEmail(email);
            }

            _contactRepository.Add(contact);
            await _unitOfWork.SaveAsync(); 
        }

        public async Task UpdateContactAsync(UpdateContactDTO dto)
        {
            var contact = await _contactRepository.GetByIdAsync(dto.Id);
            if (contact == null)
            {
                throw new ServiceException($"Kontakt o ID: {dto.Id} nie istnieje.");
            }

            var firstName = new FirstName(dto.FirstName);
            var lastName = new LastName(dto.LastName);
            var age = new Age(dto.Age);

            contact.UpdateBasicInfo(firstName, lastName, dto.Sex, age);

            await _unitOfWork.SaveAsync();
        }

        public async Task RemoveContactAsync(int contactId)
        {
            var contact = await _contactRepository.GetByIdAsync(contactId);
            if (contact == null)
            {
                throw new ServiceException($"Kontakt o ID: {contactId} nie istnieje.");
            }

            _contactRepository.Remove(contact);
            
            await _unitOfWork.SaveAsync();
        }
    }
}