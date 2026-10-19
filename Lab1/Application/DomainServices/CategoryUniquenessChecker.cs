using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DomainServices
{
    public class CategoryUniquenessChecker
    {
        public Result<Unit> EnsureUnique(string name) => categoryRepository.GetByName(name).Match(_ => Result.Failure<Unit>($"Kategoria o nazwie '{name}' już istnieje"), () => Result.Success(Unit.Instance));
    }
}