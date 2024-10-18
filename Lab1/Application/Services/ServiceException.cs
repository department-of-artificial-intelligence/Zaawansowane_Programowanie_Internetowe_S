namespace Contacts.Application.Services;

public class ServiceException(string message)
    : ApplicationException(message)
    {
    }