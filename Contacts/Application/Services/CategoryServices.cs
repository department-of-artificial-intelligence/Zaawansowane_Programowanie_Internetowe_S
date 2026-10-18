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
            .Bind(category => {
                bool isCategoryInUse = contactRepository.GetAll()
                    .SelectMany(c=>c.Emails)
                    .Any(e => e.CategoryId == id);
                if(isCategoryInUse){
                    return Result.Failure<Category>("Nie mozna usunąć kategorii");
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
                categoryUniquenessChecker
                    .EnsureUnique(updateCategoryDto.Name)
                    .Map(_ => category)
            )
            .Tap(category => category.Rename(updateCategoryDto.Name))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
}