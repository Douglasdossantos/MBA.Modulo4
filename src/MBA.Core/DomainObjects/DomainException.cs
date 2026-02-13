using MBA.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MBA.Core.DomainObjects
{
    public class DomainException: Exception
    {
        public IReadOnlyCollection<string> Errors { get; }

        public DomainException() { }

        public DomainException(string message) : base(message) 
        {
            Errors = [message];
        }
        public DomainException(string? message, Exception? innerException) : base(message, innerException) {}


        public DomainException(IEnumerable<string> mensagens) : base(string.Join("; ", mensagens))
        {
            Errors = mensagens.ToList().AsReadOnly();
        }
    }
}
