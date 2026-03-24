using System.ComponentModel.DataAnnotations;

namespace MBA.Conteudo.Application.ViewModels
{
    public class CursoViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public bool Ativo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public DateTime Validade { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Finalidade { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Ementa { get; set; }

    }
}
