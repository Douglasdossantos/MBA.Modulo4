namespace MBA.API.ViewModels
{
    public class CadastroCursoViewModel
    {
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public DateTime? ValidoAte { get; set; }

        public string Finalidade { get; set; }
        public string Ementa { get; set; }
    }
}
