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
            // normalize authorization header
            string authHeader = null;
            if (!string.IsNullOrWhiteSpace(authorization))
            {
                authHeader = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? authorization : $"Bearer {authorization}";
            }

            var requestCadastro = await CadastrarCurso(matriculaViewModel, authHeader);

            var requestPagamento = await AlunoRealizaPagamento(matriculaViewModel.AlunoId, new RealizarPagamentoRequest
            {
                AlunoId = matriculaViewModel.AlunoId,   
                CursoId = matriculaViewModel.CursoId,
                MatriculaCursoId = matriculaViewModel.MatriculaCursoId != Guid.Empty ? matriculaViewModel.MatriculaCursoId : matriculaViewModel.CursoId,
                PagamentoPodeSerRealizado = requestCadastro.StatusCode == 200, // pagamento só se matrícula ok
                NomeCurso = matriculaViewModel.NomeCurso, // Você pode obter o nome do curso de outra fonte se necessário
                DataMatricula = DateTime.UtcNow,
                Valor = matriculaViewModel.Valor,
                CvvCartao = matriculaViewModel.CvvCartao,
                NomeTitularCartao = matriculaViewModel.NomeTitularCartao,   
                NumeroCartao = matriculaViewModel.NumeroCartao, 
                ValidadeCartao = matriculaViewModel.ValidadeCartao

            }, requestCadastro.AuthHeader);

            if (requestPagamento == null)
            {
                return new ContentResult
                {
                    Content = string.Empty,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            var paymentContent = await requestPagamento.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = paymentContent,
                ContentType = requestPagamento.Content.Headers.ContentType?.ToString() ?? "application/json",
                StatusCode = (int)requestPagamento.StatusCode
            };
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

        public async Task<HttpResponseMessage> AlunoRealizaPagamento(Guid alunoId, RealizarPagamentoRequest matriculaViewModel, string authorization)
        {
            // normalize authorization header
            string authHeader = null;
            if (!string.IsNullOrWhiteSpace(authorization))
            {
                authHeader = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? authorization : $"Bearer {authorization}";
            }

            try
            {
                var response = await _faturamentoExternalService.RealizarPagamento(alunoId, matriculaViewModel, authHeader);

                if (response == null)
                    return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                    {
                        Content = new System.Net.Http.StringContent(string.Empty)
                    };

                return response;
            }
            catch (Refit.ApiException ex)
            {
                var status = ex.StatusCode != default ? ex.StatusCode : System.Net.HttpStatusCode.InternalServerError;
                var content = ex.Content ?? ex.Message;
                return new HttpResponseMessage(status)
                {
                    Content = new System.Net.Http.StringContent(content)
                };
            }
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
