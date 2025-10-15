namespace Contacts.Application.Services
{
    public class ServiceException : ApplicationException
    {
        public ServiceException(string message) : base(message) { }
    }
}
