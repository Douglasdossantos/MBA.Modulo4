using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Core.Messages
{
    public abstract class Command : Message, IRequest<ValidationResult>
    {
        protected Command()
        {
            TimeStamp = DateTime.Now;
        }

        public DateTime TimeStamp { get; private set; }
        public ValidationResult ValidationResult { get; set; }

        public virtual bool Valido()
        {
            throw new NotImplementedException();
        }
    }
}
