using FluentValidation.Results;
using MBA.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Core.Messages
{
    public abstract class CommandHandler
    {
        protected ValidationResult ValidationResult;

        protected CommandHandler()
        {
            ValidationResult = new ValidationResult();
        }

        protected void AdicionarErro(String messagem)
        {
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, messagem));
        }

        protected async Task<ValidationResult> PersistirDados(IUnitOfWork uow)
        {
            if (!await uow.Commit())
            {
                AdicionarErro("Houve um erro ao persistir os dados");
            }
            return ValidationResult;
        }
    }
}
