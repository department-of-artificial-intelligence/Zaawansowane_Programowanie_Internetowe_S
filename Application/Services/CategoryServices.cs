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

namespace Application.Services;

public class CategoryServices(
    ICategoryRepository categoryRepository,
    IContactRepository contactRepository,
    IUnitOfWork unitOfWork,
    CategoryUniquenessChecker categoryUniquenessChecker
) : ICategoryUseCases
{
    public Result<AddCategoryResult> AddCategory(AddCategoryDto addCategoryDto)
        => addCategoryDto
        .ToDomain()
        .Bind(category =>
            categoryUniquenessChecker
            .EnsureUnique(category.Name)
            .Map(_ => category)
        )
        .Tap(categoryRepository.Add)
        .Tap(_ => unitOfWork.Save())
        .Map(AddCategoryResult.FromDomain);

    public Result<Unit> RemoveCategory(Guid id)
        => categoryRepository.GetById(id)
            .Required($"Nie mogę odnaleźć kategorii o id = {id}")
            .Bind(category =>
            {
                //Czy kontakt używa tej kategorii w swoich adresach email?
                bool isCategoryInUse = contactRepository.GetAll()
                    .Any(contact => contact.Emails.Any(email => email.CategoryId == id));

                if (isCategoryInUse)
                {
                    return Result.Failure<Category>("Nie można usunąć kategorii, ponieważ jest ona przypisana do co najmniej jednego adresu email.");
                }

                return Result.Success(category);
            })
            .Tap(categoryRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        => categoryRepository.GetById(updateCategoryDto.Id)
        .Required($"Nie mogę odnaleźć kategorii o id = {updateCategoryDto.Id}")
        .Bind(category =>
            categoryUniquenessChecker.EnsureUnique(updateCategoryDto.Name)
                .Map(_ => category)
        )
        .Bind(category => category.Rename(updateCategoryDto.Name))
        .Tap(_ => unitOfWork.Save());
}
