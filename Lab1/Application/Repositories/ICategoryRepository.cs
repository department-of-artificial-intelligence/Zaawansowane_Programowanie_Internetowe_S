using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Domain;

namespace Application.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        option<Category> GetById(Guid categoryId);
        option<Category> GetByName(string categoryName);
        void Add(Category category);
        void Remove (Category category);
    }
}