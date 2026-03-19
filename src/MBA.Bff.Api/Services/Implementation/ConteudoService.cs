using MBA.Bff.Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Services.Implementation
{
    public class ConteudoService(IConteudoExternalServiceService conteudoService): IConteudoService
    {
        private readonly IConteudoExternalServiceService _conteudoService = conteudoService;
        
        
        public async Task<IActionResult> ObterAulaPorId(Guid aulaId)
        {
            var response = await _conteudoService.ObterAulaPorId(aulaId);

            if (response == null)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            var content = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = content,
                StatusCode = (int)response.StatusCode,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json"
            };
        }
    }
}
