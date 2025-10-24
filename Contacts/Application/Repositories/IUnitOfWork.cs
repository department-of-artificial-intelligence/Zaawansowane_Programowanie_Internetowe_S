using System.Threading.Tasks; 

namespace Contacts.Application.Repositories
{
    public interface IUnitOfWork
    {
        void Save();
        Task SaveAsync(); 
    }
}