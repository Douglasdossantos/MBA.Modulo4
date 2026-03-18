using System.ComponentModel.DataAnnotations;

namespace MBA.Conteudo.Api.ViewModels
{
    public class AtualizacaoCursoViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(200, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 3)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O campo {0} deve ser maior que {1}")]
        public decimal Valor { get; set; }

        public DateTime? ValidoAte { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public ConteudoProgramaticoViewModel ConteudoProgramatico { get; set; } = new ConteudoProgramaticoViewModel();
    }
}
