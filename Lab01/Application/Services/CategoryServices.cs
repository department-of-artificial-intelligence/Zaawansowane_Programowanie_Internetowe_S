using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;

namespace Contacts. Application.Services;
public class CategoryServices (
ICategoryRepository categoryRepository,
IUnitOfWork unitOfWork
): ICategoryUseCases
{
public void AddCategory (AddCategoryDTO categoryDTO)
{
if (categoryRepository.GetCategoryByName(categoryDTO.Name)
{
categoryRepository. Add (categoryDTO); unitofwork.Save();
}
== null)
else
throw new ServiceException("Kategoria_o_tej_nazwie_już_istnieje");
{
}
}
public void RemoveCategory(int categoryID)
{
var category = categoryRepository.GetCategory(categoryID); if (category != null)
{
categoryRepository.RemoveCategory(category);
unitofwork.Save();
}
else
{
}
}
throw new ServiceException(
$"Kategoria_o_id:_{categoryID}_nie_istnieje");