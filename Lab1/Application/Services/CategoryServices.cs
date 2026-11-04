using Application.Domain;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
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
.EnsureUnique(category.Name, category.UserId)
.Map(_ => category)
)
.Tap(categoryRepository.Add)
.Tap(_ => unitOfWork.Save())
.Map(AddCategoryResult.FromDomain);
public Result<Unit> RenameCategory(RenameCategoryDto dto)
=> categoryRepository.GetById(dto.Id)
.Required($"Nie mogę odnaleźć kategorii o id = {dto.Id}")
.Bind(category => category.Rename(dto.Name))
.Tap(_ => unitOfWork.Save());
public Result<Unit> RemoveCategory(Guid id)
=> categoryRepository.GetById(id)
.Required($"Nie mogę odnaleźć kategorii o id = {id}")
.Bind(category => categoryRepository.HasAssignedEmails(id)
? Result.Failure<Category>(
    "Nie można usunąć kategorii przypisanej do adresu email.")
: Result.Success(category))
.Tap(categoryRepository.Remove)
.Tap(_ => unitOfWork.Save())
.Map(_ => Unit.Instance);
}
