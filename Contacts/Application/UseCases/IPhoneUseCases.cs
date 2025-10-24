using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.UseCases;

public interface IPhoneUseCases
{
    AddPhoneResult AddPhone(AddPhoneDTO dto);
    void RemovePhone(int phoneId);
}