using Contacts.Application.Domain;
using Contacts.Application.Repositories;
using Contacts.Application.UseCases.DTOs;
using System.Threading.Tasks;

namespace Contacts.Application.Services
{
    public class ContactServices : IContactUseCases
    {
        // Repozytoria (bez zmian)
        private readonly IContactRepository _contactRepository;
        private readonly ICategoryRepository _categoryRepository; 
        private readonly IUnitOfWork _unitOfWork;

        // Konstruktor (bez zmian)
        public ContactServices(
            IContactRepository contactRepository,
            ICategoryRepository categoryRepository, 
            IUnitOfWork unitOfWork)
        {
            _contactRepository = contactRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        // --- Metody dla Kontaktu (bez zmian) ---
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
                    throw new ServiceException($"Kategoria o ID: {emailDto.CategoryId} nie istnieje.");
                
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
                throw new ServiceException($"Kontakt o ID: {dto.Id} nie istnieje.");

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
                throw new ServiceException($"Kontakt o ID: {contactId} nie istnieje.");

            _contactRepository.Remove(contact);
            await _unitOfWork.SaveAsync();
        }

        // --- Metody dla Email (bez zmian) ---
        public async Task AddEmailToContactAsync(AddEmailToContactDTO dto)
        {
            var contact = await _contactRepository.GetByIdAsync(dto.ContactId);
            if (contact == null)
                throw new ServiceException($"Kontakt o ID: {dto.ContactId} nie istnieje.");

            var category = _categoryRepository.GetCategory(dto.CategoryId);
            if (category == null)
                throw new ServiceException($"Kategoria o ID: {dto.CategoryId} nie istnieje.");

            var emailAddress = new EmailAddress(dto.Address);
            var email = new Email(emailAddress, category.Id);
            contact.AddEmail(email);
            await _unitOfWork.SaveAsync();
        }

        public async Task RemoveEmailFromContactAsync(RemoveEmailFromContactDTO dto)
        {
            var contact = await _contactRepository.GetByIdAsync(dto.ContactId);
            if (contact == null)
                throw new ServiceException($"Kontakt o ID: {dto.ContactId} nie istnieje.");

            var email = contact.FindEmailById(dto.EmailId);
            if (email == null)
                throw new ServiceException($"Email o ID: {dto.EmailId} nie został znaleziony dla tego kontaktu.");
            
            contact.RemoveEmail(email);
            await _unitOfWork.SaveAsync();
        }
        
        // --- Metody dla Telefonów (z poprawką błędu) ---
        public async Task AddPhoneToContactAsync(AddPhoneToContactDTO dto)
        {
            var contact = await _contactRepository.GetByIdAsync(dto.ContactId);
            if (contact == null)
                throw new ServiceException($"Kontakt o ID: {dto.ContactId} nie istnieje.");

            var category = _categoryRepository.GetCategory(dto.CategoryId);
            if (category == null)
                throw new ServiceException($"Kategoria o ID: {dto.CategoryId} nie istnieje.");

            // POPRAWKA BŁĘDU: Z 'PhoneNumber' na 'PhoneNumberVO'
            var phoneNumber = new PhoneNumberVO(dto.Number); 
            var phone = new Phone(phoneNumber, category.Id);

            contact.AddPhone(phone);
            await _unitOfWork.SaveAsync();
        }

        public async Task RemovePhoneFromContactAsync(RemovePhoneFromContactDTO dto)
        {
            var contact = await _contactRepository.GetByIdAsync(dto.ContactId);
            if (contact == null)
                throw new ServiceException($"Kontakt o ID: {dto.ContactId} nie istnieje.");

            var phone = contact.FindPhoneById(dto.PhoneId);
            if (phone == null)
                throw new ServiceException($"Telefon o ID: {dto.PhoneId} nie został znaleziony dla tego kontaktu.");
            
            contact.RemovePhone(phone);
            await _unitOfWork.SaveAsync();
        }

        // --- NOWE METODY DLA WAŻNYCH DAT ---
        public async Task AddImportantDateToContactAsync(AddImportantDateToContactDTO dto)
        {
            // 1. Pobieramy agregat
            var contact = await _contactRepository.GetByIdAsync(dto.ContactId);
            if (contact == null)
                throw new ServiceException($"Kontakt o ID: {dto.ContactId} nie istnieje.");
            
            // 2. Tworzymy VO (walidacja opisu)
            var description = new DateDescriptionVO(dto.Description);
            
            // 3. Tworzymy encję
            var importantDate = new ImportantDate(dto.Date, description);

            // 4. Wywołujemy metodę domenową
            contact.AddImportantDate(importantDate);
            
            // 5. Zapisujemy zmiany
            await _unitOfWork.SaveAsync();
        }

        public async Task RemoveImportantDateFromContactAsync(RemoveImportantDateFromContactDTO dto)
        {
            // 1. Pobieramy agregat
            var contact = await _contactRepository.GetByIdAsync(dto.ContactId);
            if (contact == null)
                throw new ServiceException($"Kontakt o ID: {dto.ContactId} nie istnieje.");

            // 2. Znajdujemy datę w kolekcji
            var importantDate = contact.FindImportantDateById(dto.ImportantDateId);
            if (importantDate == null)
                throw new ServiceException($"Ważna data o ID: {dto.ImportantDateId} nie została znaleziona dla tego kontaktu.");

            // 3. Wywołujemy metodę domenową
            contact.RemoveImportantDate(importantDate);
            
            // 4. Zapisujemy zmiany
            await _unitOfWork.SaveAsync();
        }
    }
}