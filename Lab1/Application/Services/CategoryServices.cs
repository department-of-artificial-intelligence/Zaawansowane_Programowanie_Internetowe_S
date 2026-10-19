using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryServices(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, CategorUniquenessChecker categorUniquenessChecker) : ICategoryUseCases
    {
        public Result<AddCategoryResult> AddCategory(AddCategoryDto addCategoryDto) => addCategoryDto.ToDomain().Bind(category => categorUniquenessChecker.EnsureUnique(category.Name).Map(_ => category)).Tap(categoryRepository.Add).Tap(_ => unitOfWork.Save()).Map(AddCategoryResult.FromDomain);

        public Result <Unit> RemoveCategory(Guid id) => categoryRepository.GetById(id).Required($"Nie mogę odnaleźć kategorii o id = {id}").Tap(categoryRepository.Remove).Tap(_ => unitOfWork.Save()).Map(_ => Unit.Instance);
    }
}