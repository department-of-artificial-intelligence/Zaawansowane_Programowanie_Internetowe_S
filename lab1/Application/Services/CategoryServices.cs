
using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
using Application.Domain;

namespace Application.Services;

public class CategoryServices(
    ICategoryRepository categoryRepository,
    IContactRepository contactRepository,
    IUnitOfWork unitOfWork,
    CategoryUniquenessChecker categoryUniquenessChecker
) : ICategoryUseCases
{
    public Result<AddCategoryResult> AddCategory(
        AddCategoryDto addCategoryDto)
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
                bool isUsed = contactRepository.GetAll()
                    .Any(contact =>
                        contact.Emails.Any(email =>
                            email.CategoryId == id));

                if (isUsed)
                {
                    return Result.Failure<Category>(
                        "Nie można usunąć kategorii, ponieważ jest przypisana do adresu e-mail.");
                }

                return Result.Success(category);
            })
            .Tap(categoryRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> UpdateCategory(
        UpdateCategoryDto updateCategoryDto)
        => categoryRepository.GetById(updateCategoryDto.Id)
            .Required(
                $"Nie mogę odnaleźć kategorii o id = {updateCategoryDto.Id}")
            .Bind(category =>
                categoryUniquenessChecker
                    .EnsureUnique(
                        updateCategoryDto.Name,
                        updateCategoryDto.Id)
                    .Bind(_ => category.Rename(updateCategoryDto.Name))
                    .Map(_ => category)
            )
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
}
