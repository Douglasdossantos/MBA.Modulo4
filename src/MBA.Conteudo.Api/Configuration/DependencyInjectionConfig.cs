using MBA.Conteudo.Api.Data;
using MBA.Conteudo.Api.Data.Repository;
using MBA.Conteudo.Api.Models.Interfaces;
using MBA.Conteudo.Api.Services;
using MBA.Conteudo.Api.Services.Interfaces;
using MBA.Core.Autentications;
using MBA.Core.DomainHadlers;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.WebApi.Core.Usuario;
using MediatR;

namespace MBA.Conteudo.Api.Configuration
{
    public class DependencyInjectionConfig
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IConteudoRepository, ConteudoRepository>();
            services.AddScoped<ConteudoContext>();

            // Application Services
            services.AddScoped<ICursoAppService, CursoAppService>();
            services.AddScoped<IAulaAppService, AulaAppService>();

            // User
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            // AutoMapper
                services.AddAutoMapper(typeof(DependencyInjectionConfig).Assembly);

            services.AddScoped<IAppIdentityUser, AppIdentityUser>();

            // Mediator (core notifications/comandos/eventos de domínio)
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatorHandler).Assembly));
                services.AddScoped<IMediatorHandler, MediatorHandler>();
                services.AddScoped<INotificationHandler<DomainNotificacaoRaiz>, DomainNotificacaoHandler>();
        }
    }
}