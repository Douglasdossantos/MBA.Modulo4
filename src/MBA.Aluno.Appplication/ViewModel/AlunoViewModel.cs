using System.ComponentModel.DataAnnotations;

namespace MBA.Aluno.Appplication.ViewModel
{
    public class AlunoViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public bool Ativo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public bool Adm { get; set; }
    }
}
