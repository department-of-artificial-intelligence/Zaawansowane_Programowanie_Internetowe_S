using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.UseCases;

public interface IEmailUseCases
{
    AddEmailResult AddEmail(AddEmailDTO dto);
    void RemoveEmail(int emailId);
}