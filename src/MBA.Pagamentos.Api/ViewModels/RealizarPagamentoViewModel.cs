using MBA.Core.SharedDto;

namespace MBA.Pagamentos.Api.ViewModels
{
    public class RealizarPagamentoViewModel
    {
        public Guid AlunoId { get; set; }
        public Guid CursoId { get; set; }
        public Guid MatriculaCursoId { get; set; }
        public bool PagamentoPodeSerRealizado { get; set; }
        public string NomeCurso { get; set; }
        public DateTime DataMatricula { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string EstadoMatricula { get; set; }
        public decimal Valor { get; set; }
        public string NumeroCartao { get; set; }
        public string NomeTitularCartao { get; set; }
        public string ValidadeCartao { get; set; }
        public string CvvCartao { get; set; }
    }
}
