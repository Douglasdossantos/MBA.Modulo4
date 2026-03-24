using System;

namespace MBA.Bff.Api.Models.Conteudo
{
    // DTO sent to Conteudo API (excludes Login property)
    public class CadastrarCursoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime? ValidoAte { get; set; }
        public ConteudoProgramaticoViewModel ConteudoProgramatico { get; set; } = new ConteudoProgramaticoViewModel();
    }
}