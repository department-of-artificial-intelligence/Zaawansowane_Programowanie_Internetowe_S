using Application.Domain;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryServices(
        ICategoryRepository categoryRepository,
        IContactRepository contactRepository, // <-- Dodany parametr
        IUnitOfWork unitOfWork,
        CategoryUniquenessChecker categoryUniquenessChecker
    ) : ICategoryUseCases
    {
        public Result<AddCategoryResult> AddCategory(AddCategoryDto addCategoryDto) =>
     addCategoryDto
         .ToDomain()
         .Bind(category => categoryUniquenessChecker
             .EnsureUnique(category.Name, category.UserId)
             .Map(_ => category))
         .Tap(categoryRepository.Add)
         .Tap(_ => unitOfWork.Save())
         .Map(AddCategoryResult.FromDomain);

        public Result<Unit> UpdateCategory(UpdateCategoryDto updateCategoryDto)
            => categoryRepository.GetById(updateCategoryDto.Id)
                .Required($"Nie mogę odnaleźć kategorii o id {updateCategoryDto.Id}")
                .Bind(category =>
                    category.Name == updateCategoryDto.Name
                        ? Result.Success(category)
                        : categoryUniquenessChecker
                            .EnsureUnique(category.Name, category.UserId)
                            .Map(_ => category))
                .Bind(category => category.Rename(updateCategoryDto.Name))
                .Tap(_ => unitOfWork.Save());

        public Result<Unit> RemoveCategory(Guid id)
            => categoryRepository.GetById(id)
                .Required($"Nie mogę odnaleźć kategorii o id {id}")
                .Bind(category =>
                    contactRepository.IsCategoryInUse(category.Id)
                        ? Result.Failure<Category>("Nie można usunąć kategorii, ponieważ jest przypisana do istniejących adresów e-mail.")
                        : Result.Success(category))
                .Tap(categoryRepository.Remove)
                .Tap(_ => unitOfWork.Save())
                .Map(_ => Unit.Instance);
    }

}
