using MBA.Conteudo.Application.ViewModels;
using MBA.Conteudo.Domain.Entities;
using MBA.Conteudo.Domain.Interfaces;
using MBA.Conteudo.Domain.ValueObjects;
using MBA.Core.DomainObjects;
using MBA.Core.SharedDto;

namespace MBA.Conteudo.Application.Services;

public class CursoAppService(IConteudoRepository cursoRepository) : ICursoAppService
{
	public async Task<Guid> CadastrarCursoAsync(CadastroCursoDto dto)
	{
		if (await cursoRepository.ExisteCursoComMesmoNomeAsync(dto.Nome))
			throw new DomainException("Já existe um curso cadastrado com esse nome.");

		var curso = new Curso(dto.Nome,
			dto.Valor,
			dto.ValidoAte,
			new ConteudoProgramatico(dto.Finalidade, dto.Ementa));

		await cursoRepository.AdicionarAsync(curso);
		await cursoRepository.UnitOfWork.Commit();
		return curso.Id;
	}

	public async Task AtualizarCursoAsync(Guid cursoId, AtualizacaoCursoDto dto)
	{
		var curso = await cursoRepository.ObterPorIdAsync(cursoId) ?? throw new DomainException("Curso não encontrado");

		if (!curso.Nome.Equals(dto.Nome, StringComparison.OrdinalIgnoreCase) &&
			await cursoRepository.ExisteCursoComMesmoNomeAsync(dto.Nome))
			throw new DomainException("Já existe um curso cadastrado com esse nome.");

		curso.AlterarNome(dto.Nome);
		curso.AlterarValor(dto.Valor);
		curso.AlterarValidadeCurso(dto.ValidoAte);

		curso.AtualizarConteudoProgramatico(dto.Finalidade, dto.Ementa);
		if (dto.Ativo)
			curso.AtivarCurso();
		else
			curso.DesativarCurso();

		await cursoRepository.AtualizarAsync(curso);
		await cursoRepository.UnitOfWork.Commit();
	}

	public async Task DesativarCursoAsync(Guid cursoId)
	{
		var curso = await cursoRepository.ObterPorIdAsync(cursoId) ?? throw new DomainException("Curso não encontrado");

		curso.DesativarCurso();
		await cursoRepository.AtualizarAsync(curso);
		await cursoRepository.UnitOfWork.Commit();
	}

	public async Task<CursoDto> ObterPorIdAsync(Guid cursoId)
	{
		var curso = await cursoRepository.ObterPorIdAsync(cursoId) ?? throw new DomainException("Curso não encontrado");
		return MapearParaCursoDto(curso);
	}

	public async Task<IEnumerable<CursoDto>> ObterTodosAsync()
	{
		var cursos = await cursoRepository.ObterTodosAsync();
		return cursos.Select(MapearParaCursoDto);
	}

	public async Task<IEnumerable<CursoDto>> ObterAtivosAsync()
	{
		var cursos = await cursoRepository.ObterAtivosAsync();
		return cursos.Select(MapearParaCursoDto);
	}

	#region Mapeamento Manual

	private CursoDto MapearParaCursoDto(Curso curso)
	{
		return new CursoDto
		{
			Id = curso.Id,
			Nome = curso.Nome,
			Valor = curso.Valor,
			Finalidade = curso.ConteudoProgramatico.Finalidade,
			Ementa = curso.ConteudoProgramatico.Ementa,
			CursoDisponivel = curso.CursoDisponivel(),
			CargaHoraria = curso.CargaHoraria(),
			QuantidadeAulas = curso.QuantidadeAulas(),
			Aulas = curso.Aulas.Select(a => new AulaDto
			{
				Id = a.Id,
				Descricao = a.Descricao,
				Ativo = a.Ativo,
				CargaHoraria = a.CargaHoraria,
				OrdemAula = a.OrdemAula,
				Url = a.Url
			}).OrderBy(x => x.OrdemAula)
				.ToList()
		};
	}

	#endregion
}