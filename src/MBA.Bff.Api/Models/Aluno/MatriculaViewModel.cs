using MBA.Bff.Api.Models.Autenticacao;
using System.ComponentModel.DataAnnotations;

namespace MBA.Bff.Api.Models.Aluno
{
    public class MatriculaViewModel
    {
        public UsuarioLoginViewModel Login { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid CursoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid AlunoId { get; set; }
    }
}