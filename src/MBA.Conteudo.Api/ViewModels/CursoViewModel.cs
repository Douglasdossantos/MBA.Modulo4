namespace MBA.Conteudo.Api.ViewModels
{
    public class CursoViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public bool Ativo { get; set; }
        public DateTime? ValidoAte { get; set; }
        public short CargaHoraria { get; set; }
        public int QuantidadeAulas { get; set; }
        public ConteudoProgramaticoViewModel ConteudoProgramatico { get; set; }
        public List<AulaResultViewModel> Aulas { get; set; }
    }

    public class AulaResultViewModel
    {
        public Guid Id { get; set; }
        public Guid CursoId { get; set; }
        public string Descricao { get; set; }
        public short CargaHoraria { get; set; }
        public byte OrdemAula { get; set; }
        public string Url { get; set; }
        public bool Ativo { get; set; }
    }
}
