using System.ComponentModel.DataAnnotations;

namespace MBA.Conteudo.Api.ViewModels
{
    public class AdicionarAulaViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid CursoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 5)]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(1, 5, ErrorMessage = "O campo {0} deve estar entre {1} e {2}")]
        public short CargaHoraria { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(1, 255, ErrorMessage = "O campo {0} deve estar entre {1} e {2}")]
        public byte OrdemAula { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(1024, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 10)]
        [Url(ErrorMessage = "O campo {0} deve ser uma URL válida")]
        public string Url { get; set; } = string.Empty;
    }

    public class AtualizarAulaViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid CursoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 5)]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(1, 5, ErrorMessage = "O campo {0} deve estar entre {1} e {2}")]
        public short CargaHoraria { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(1, 255, ErrorMessage = "O campo {0} deve estar entre {1} e {2}")]
        public byte OrdemAula { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(1024, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 10)]
        [Url(ErrorMessage = "O campo {0} deve ser uma URL válida")]
        public string Url { get; set; }
    }
}
