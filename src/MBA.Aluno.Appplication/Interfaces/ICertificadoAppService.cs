namespace MBA.Aluno.Application.Interfaces;

public interface ICertificadoAppService
{
	Task<Guid> CadastrarCertificadoAsync(Guid matriculaId);
}