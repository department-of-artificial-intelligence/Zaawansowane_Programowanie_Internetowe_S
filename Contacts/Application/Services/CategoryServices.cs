
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Domain;

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
                var isCategoryInUse = contactRepository.GetAll()
                    .Any(contact => contact.Emails.Any(email => email.CategoryId == id));


                return isCategoryInUse
                    ? Result.Failure<Category>($"Nie można usunąć kategorii '{category.Name}', ponieważ jest przypisana do istniejącego adresu e-mail.")
                    : Result.Success(category);
            })
                .Tap(categoryRepository.Remove)
                .Tap(_ => unitOfWork.Save())
                .Map(_ => Unit.Instance);


    public Result<Unit> UpdateCategory(Guid id, string newName)
        => categoryRepository.GetById(id)
            .Required($"Nie mogę odnaleźć kategorii o id = {id}")
            .Bind(category =>
                categoryUniquenessChecker.EnsureUnique(newName)
                    .Bind(_ => category.UpdateName(newName)))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

}