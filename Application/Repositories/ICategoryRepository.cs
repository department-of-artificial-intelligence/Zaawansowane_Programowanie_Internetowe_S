using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface ICategoryRepository
{
    IEnumerable<Category> GetAll();
    Option<Category> GetById(Guid categoryId);
    Option<Category> GetByName(string categoryName);
    void Add(Category category);
    void Remove(Category category);
}

