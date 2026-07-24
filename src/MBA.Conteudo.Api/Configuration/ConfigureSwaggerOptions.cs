using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MBA.Conteudo.Api.Configuration;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
	private const string BannerSwaggerExposto =
		"AVISO IMPORTANTE — SWAGGER EXPOSTO DE PROPÓSITO: Esta é uma aplicação acadêmica (MBA DevXpert, Módulo 5) avaliada por professores, e TODOS os ambientes (inclusive produção) expõem esta documentação para facilitar a consulta e a correção do trabalho. A equipe SABE que em uma aplicação real o Swagger NÃO deve ficar público em produção. Para ocultá-lo, por padrao o Swagger fica oculto em ambiente publicado; para exibi-lo defina SWAGGER_ENABLED=true.";

	private readonly IApiVersionDescriptionProvider _provider;

	public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
	{
		_provider = provider;
	}

	public void Configure(SwaggerGenOptions options)
	{
		foreach (var description in _provider.ApiVersionDescriptions)
			options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
	}

	private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
	{
		var info = new OpenApiInfo
		{
			Title = "API - desenvolvedor.io",
			Version = description.ApiVersion.ToString(),
			Description = BannerSwaggerExposto + "\n\nEsta API faz parte do curso REST com ASP.NET Core WebAPI.",
			Contact = new OpenApiContact { Name = "Eduardo Pires", Email = "contato@desenvolvedor.io" },
			License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
		};

		if (description.IsDeprecated) info.Description += " Esta versão está obsoleta!";

		return info;
	}
}