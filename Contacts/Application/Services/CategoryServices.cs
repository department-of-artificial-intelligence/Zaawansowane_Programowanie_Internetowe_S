
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

            .Tap(categoryRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> UpdateCategory(Guid id, string newName)
        => categoryRepository.GetById(id)
            .Required($"Nie mogę odnaleźć kategorii o id = {id}")
            .Bind(category => category.UpdateName(newName))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

}