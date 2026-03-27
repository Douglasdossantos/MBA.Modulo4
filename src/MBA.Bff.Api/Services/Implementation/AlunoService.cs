using MBA.Bff.Api.Models;
using MBA.Bff.Api.Models.Aluno;
using MBA.Bff.Api.Models.Pagamento;
using MBA.Bff.Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Services.Implementation
{
    public class AlunoService(IAlunoExternalService alunoExternalService,
        IFaturamentoExternalService faturamentoExternalService) : IAlunoService
    {
        private readonly IAlunoExternalService _alunoExternalService = alunoExternalService;
        private readonly IFaturamentoExternalService _faturamentoExternalService = faturamentoExternalService;


        public async Task<IActionResult> MatriculaPagamento(MatriculaViewModel matriculaViewModel, string authorization)
        {
            var requestCadastro = await CadastrarCurso(matriculaViewModel, authorization);

            var requestPagamento = await AlunoRealizaPagamento(matriculaViewModel.AlunoId, new RealizarPagamentoRequest
            {
                CursoId = matriculaViewModel.CursoId,
                Valor = 1000 // Exemplo de valor, você pode ajustar conforme necessário
            }, requestCadastro.AuthHeader);

            return requestCadastro;
        }

        private async Task<ContentCustomResult> CadastrarCurso(MatriculaViewModel matriculaViewModel, string authorization)
        {
            string authHeader = null;
            if (!string.IsNullOrWhiteSpace(authorization))
            {
                authHeader = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? authorization : $"Bearer {authorization}";
            }

            var request = new MatriculaRequest
            {
                AlunoId = matriculaViewModel.AlunoId,
                CursoId = matriculaViewModel.CursoId,
            };

            var response = await _alunoExternalService.MatricularAluno(request, authHeader);

            if (response == null)
                return new ContentCustomResult();

            var content = await response.Content.ReadAsStringAsync();

            return new ContentCustomResult
            {
                Content = content,
                AuthHeader = authHeader,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        public async Task<ContentResult> AlunoRealizaPagamento(Guid alunoId, RealizarPagamentoRequest matriculaViewModel, string authorization)
        {
            var response = await _faturamentoExternalService.RealizarPagamento(alunoId, matriculaViewModel, authorization);

            if (response == null)
                return new ContentResult();

            var content = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = content,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        public async Task<ContentResult> RealizarAula(AulaAssistidaViewModel aulaAssistidaViewModel, string authorization)
        {
            // normalize authorization header expected by external services
            string authHeader = null;
            if (!string.IsNullOrWhiteSpace(authorization))
            {
                authHeader = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? authorization : $"Bearer {authorization}";
            }

            try
            {
                AlunoViewModel alunoViewModel = await _alunoExternalService.ObterPorId(aulaAssistidaViewModel.AlunoId, authHeader);

                var aulaAssistidaRequest = new AulaAssistidaRequest
                {
                    AlunoId = alunoViewModel.Id,
                    MatriculaId = aulaAssistidaViewModel.MatriculaId,
                    AulaId = aulaAssistidaViewModel.AulaId
                };

                var response = await _alunoExternalService.RegistrarAulaAssistida(aulaAssistidaRequest, authHeader);

                if (response == null)
                    return new ContentResult();

                var content = await response.Content.ReadAsStringAsync();

                return new ContentResult
                {
                    Content = content,
                    ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Refit.ApiException ex)
            {
                var errorContent = ex.Content ?? ex.Message;
                var status = ex.StatusCode != default ? (int)ex.StatusCode : 500;
                return new ContentResult
                {
                    Content = errorContent,
                    ContentType = "application/json",
                    StatusCode = status
                };
            }
        }
    }
}
