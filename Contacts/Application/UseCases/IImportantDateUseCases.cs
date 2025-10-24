using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.UseCases;

public interface IImportantDateUseCases
{
    AddImportantDateResult AddImportantDate(AddImportantDateDTO dto);
    void RemoveImportantDate(int importantDateId);
}