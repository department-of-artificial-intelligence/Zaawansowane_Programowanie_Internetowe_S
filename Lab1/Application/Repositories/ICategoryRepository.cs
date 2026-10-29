using Application.Domain;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        Option<Category> GetById(Guid categoryId);
        Option<Category> GetByName(string categoryName);
        Option<Category> GetByNameAndUser(string categoryName, Guid userId);
        void Add(Category category);
        void Remove(Category category);

    }
}
