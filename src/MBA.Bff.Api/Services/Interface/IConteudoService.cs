using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Services.Interface
{
    public interface IConteudoService
    {
        Task<IActionResult> ObterAulaPorId(Guid aulaId);
    }
}