using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.UseCases.DTOs;

namespace Application.UseCases
{
    public interface ICategoryUseCases
    {
        AddCategoryResult AddCategory(AddCategoryDTO addCategoryDTO);
        void RemoveCategory(int categoryId);
    }
}
