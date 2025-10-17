using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.Application.Domain;
using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.UseCases;

public interface ICategoryUseCases
{
    void AddCategory(AddCategoryDTO addCategoryDTO);
    void RemoveCategory(int categoryId);
}
