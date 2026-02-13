using MediatR;
using FluentValidation.Results;

namespace MBA.Core.Messages
{
    public class EventoRaiz : INotification
    {
        public Guid RaizAgregacao { get; internal set; }
        public DateTime DataHora { get; internal set; }
        public ValidationResult Validacao { get; internal set; }

        public EventoRaiz()
        {
            DataHora = DateTime.Now;
        }

        public void DefinirRaizAgregacao(Guid raizAgregacao)
        {
            RaizAgregacao = raizAgregacao;
        }

        public void DefinirValidacao(ValidationResult validacao)
        {
            Validacao = validacao;
        }

        public ICollection<string> Erros => Validacao?.Errors?.Select(e => e.ErrorMessage).ToList() ?? new List<string>();
        public virtual bool EhValido() => Validacao == null || Validacao.IsValid;
    }
}
