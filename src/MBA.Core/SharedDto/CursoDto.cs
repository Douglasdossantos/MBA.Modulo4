namespace MBA.Core.SharedDto
{
    public class CursoDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public bool CursoDisponivel { get; set; }
        public int CargaHoraria { get; set; }
        public int QuantidadeAulas { get; set; }
        public string Finalidade { get; set; }
        public string Ementa { get; set; }
        public List<AulaDto> Aulas { get; set; } = [];
    }
}
