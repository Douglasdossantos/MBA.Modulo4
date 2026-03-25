using System;

namespace MBA.Bff.Api.Models.Conteudo
{
    // DTO sent to Conteudo API (excludes Login property)
    public class CadastrarCursoRequest
    {
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public DateTime? ValidoAte { get; set; }

        public string Finalidade { get; set; }
        public string Ementa { get; set; }
    }
}