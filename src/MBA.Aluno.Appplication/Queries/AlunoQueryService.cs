using MBA.Aluno.Application.Interfaces;
using MBA.Aluno.Domain.Interface;
using MBA.Conteudo.Domain.Interfaces;
using MBA.Core.SharedDto.Aluno;

namespace MBA.Aluno.Application.Queries;

public class AlunoQueryService : IAlunoQuery
{
	private readonly IAlunoRepository _alunoRepository;

	private readonly IConteudoRepository _conteudoRepository;
	private readonly IMatriculaRepository _matriculaRepository;

	public AlunoQueryService(IAlunoRepository alunoRepository,
		IMatriculaRepository matriculaRepository,
		IConteudoRepository conteudoRepository)
	{
		_matriculaRepository = matriculaRepository;
		_conteudoRepository = conteudoRepository;
		_alunoRepository = alunoRepository;
	}


	public async Task<MatriculaDto?> EvolucaoCursoPorMatriculaAsync(Guid matriculaId)
	{
		var matricula = await _matriculaRepository.ObterPorIdAsync(matriculaId);
		if (matricula is null) return null;

		var aulasAssistidas = await _alunoRepository.AulasAssistidasPorMatricula(matriculaId);
		var assistidasList = aulasAssistidas.ToList();
		var curso = await _conteudoRepository.ObterPorIdAsync(matricula.CursoId);

		MatriculaDto matriculaDto = matricula;

		var totalAulas = curso?.Aulas.Count ?? 0;
		var totalAssistidas = assistidasList.Any() ? assistidasList.Count : 0;

		matriculaDto.TotalAulas = totalAulas;
		matriculaDto.AulasAssistidas = totalAssistidas;
		matriculaDto.AulasFaltantes = Math.Max(0, totalAulas - totalAssistidas);

		decimal calculo = 0;

		if (totalAulas > 0)
			calculo = (decimal)totalAssistidas / totalAulas * 100;

		matriculaDto.Porcentagem = Math.Round(calculo, 2);

		return matriculaDto;
	}
}