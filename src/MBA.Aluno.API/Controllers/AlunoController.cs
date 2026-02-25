using MBA.Aluno.API.Application.Commands;
using MBA.Aluno.API.Models;
using MBA.Core.Mediator;
using MBA.WebApi.Core.Controllers;
using MBA.WebApi.Core.Identidade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MBA.Aluno.API.Controllers
{
    [Authorize]
    public class AlunoController : MainController
    {
        private readonly IAlunoRepository _alunosRepository;
        private readonly IMediatorHandler _mediatorHandler;

        public AlunoController(IAlunoRepository alunosRepository, IMediatorHandler mediatorHandler)
        {
            _alunosRepository = alunosRepository;
            _mediatorHandler = mediatorHandler;
        }

        [HttpGet("alunos")]
        [AllowAnonymous]
        public async Task<IEnumerable<Models.Aluno>> Index()
        {
            return  await _alunosRepository.ObterTodosAlunos();
        }

        [ClaimsAuthorize("Alunos","Ler")]
        [HttpGet("alunos/{id}")]
        public async Task<Models.Aluno> Aluno(Guid id)
        {
            return await _alunosRepository.ObterAlunoId(id);
        }

        [HttpPost("alunos-cadastrar")]
        public async Task<ActionResult> CadastroAluno(Models.Aluno aluno)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           // var result = await _alunosRepository.AdicionarAluno(aluno);

            return Ok();
        }
    }
}
