using System;
using Contacts.Application.Domain;
using Contacts.Application.Repositories;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.Services;

public class ImportantDateServices(
    IImportantDateRepository importantDateRepository,
    IContactRepository contactRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork
) : IImportantDateUseCases
{
    public AddImportantDateResult AddImportantDate(AddImportantDateDTO dto)
    {
        var contact = contactRepository.GetContact(dto.ContactId)
            ?? throw new ServiceException("Kontakt o podanym ID nie istnieje");

        var category = categoryRepository.GetCategory(dto.CategoryId)
            ?? throw new ServiceException("Kategoria o podanym ID nie istnieje");

        ImportantDateInfo info;
        try
        {
            info = new ImportantDateInfo(dto.Date, dto.Description);
        }
        catch (Exception ex)
        {
            throw new ServiceException(ex.Message);
        }

        var importantDate = new ImportantDate(0, contact, category, info);
        if (contact.HasDate(importantDate))
        {
            throw new ServiceException("Taka data z tym opisem już istnieje dla kontaktu");
        }

        importantDateRepository.Add(importantDate);
        unitOfWork.Save();
        return new AddImportantDateResult { Success = true, ImportantDateId = importantDate.Id };
    }

    public void RemoveImportantDate(int importantDateId)
    {
        var date = importantDateRepository.GetDate(importantDateId)
            ?? throw new ServiceException("Ważna data o podanym ID nie istnieje");

        importantDateRepository.Remove(date);
        unitOfWork.Save();
    }
}