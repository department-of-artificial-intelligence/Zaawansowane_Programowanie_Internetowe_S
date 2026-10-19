using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos
{
    public class AddCategoryDto(string name)
    {
        public string Name {get; set;} = name;
        public Result<Category> ToDomain()=> Category.Create(Name);
    }
}