using MBA.Auth.Api.Data;
using MBA.Auth.Api.Entidades;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MBA.Auth.Api.MigrationHelp;

public static class DbMigrationHelper
{
	private static ApplicationDbContext? _identityContext;

	private static UserManager<Usuarios>? _userManager;

	public static async Task AutocarregamentoDadosAsync(WebApplication serviceScope)
	{
		var services = serviceScope.Services.CreateScope().ServiceProvider;
		await CarregamentoDadosAsync(services);
	}

	public static async Task CarregamentoDadosAsync(IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
		var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

		_identityContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		_userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuarios>>();

		// Staging (ambiente publicado de laboratório) também cria o schema e semeia os dados;
		// Production fica de fora de propósito.
		if (env.IsDevelopment() || env.IsStaging())
		{
			// As migrations existentes são específicas de SQLite (Sqlite:Autoincrement). No SQL Server
			// elas gerariam colunas Id sem IDENTITY, quebrando os inserts. Por isso, no SQL Server, o
			// schema é criado direto do modelo (EnsureCreated gera IDENTITY correto); no SQLite mantém Migrate.
			if (_identityContext.Database.IsSqlServer())
				await _identityContext.Database.EnsureCreatedAsync();
			else
				await _identityContext.Database.MigrateAsync();

			await PopularDatabaseAsync();
		}
	}

	private static async Task PopularDatabaseAsync()
	{
		if (_identityContext!.Roles.Any()) return;
		var roleAdminId = await CriarRegraAcessoAsync(_identityContext, "Administrador");
		var roleUsuarioId = await CriarRegraAcessoAsync(_identityContext, "Alunos");
		var alunoLerId = await CriarRegraAcessoAsync(_identityContext);

		await CriarUsuarioAsync("adm@adm.com", "Adm@2026!", roleAdminId, true);
		await CriarUsuarioAsync("douglas@gmail.com", "Douglas@2026", roleUsuarioId, false);
		await CriarUsuarioAsync("outro@gmail.com", "Senha@2026", alunoLerId, false);
	}

	private static async Task<string> CriarRegraAcessoAsync(ApplicationDbContext identityContext, string role)
	{
		var roleId = Guid.NewGuid().ToString();

		identityContext.Roles.Add(new IdentityRole
		{
			Id = roleId,
			Name = role,
			NormalizedName = role.ToUpperInvariant(),
			ConcurrencyStamp = DateTime.Now.ToString("O")
		});

		await identityContext.SaveChangesAsync();

		// If this is the administrator role, add default permission claims
		if (role.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
		{
			var permissions = new[] { "AD", "AT", "DS", "VI", "PG" };

			foreach (var p in permissions)
				identityContext.RoleClaims.Add(new IdentityRoleClaim<string>
				{
					RoleId = roleId,
					ClaimType = "permission",
					ClaimValue = p
				});

			await identityContext.SaveChangesAsync();
		}

		return roleId;
	}

	private static async Task<string> CriarRegraAcessoAsync(ApplicationDbContext identityContext)
	{
		if (identityContext.Roles.Any(r => r.Name == "Aluno"))
			return identityContext.Roles.First(r => r.Name == "Aluno").Id;

		var role = new IdentityRole
		{
			Id = Guid.NewGuid().ToString(),
			Name = "Aluno",
			NormalizedName = "ALUNO"
		};

		identityContext.Roles.Add(role);
		await identityContext.SaveChangesAsync();
		var permissions = new[] { "AD", "AT", "RM", "PG" };

		foreach (var p in permissions)
			identityContext.RoleClaims.Add(new IdentityRoleClaim<string>
			{
				RoleId = role.Id,
				ClaimType = "permission",
				ClaimValue = p
			});

		await identityContext.SaveChangesAsync();


		identityContext.RoleClaims.Add(new IdentityRoleClaim<string>
		{
			RoleId = role.Id,
			ClaimType = "permission",
			ClaimValue = "ler"
		});

		await identityContext.SaveChangesAsync();

		return role.Id;
	}

	private static async Task CriarUsuarioAsync(string email, string senha, string roleId, bool ehAdmin)
	{
		var identityUser = new Usuarios
		{ UserName = email, Email = email, EmailConfirmed = true, Administrador = ehAdmin };
		var result = await _userManager!.CreateAsync(identityUser, senha);

		if (result.Succeeded)
		{
			#region Roles

			_identityContext!.UserRoles.Add(new IdentityUserRole<string>
			{
				RoleId = roleId,
				UserId = identityUser.Id
			});

			await _identityContext.SaveChangesAsync();

			#endregion Roles

			//#region Data
			//Guid userId = Guid.Parse(identityUser.Id);
			//if (ehAdmin)
			//{
			//    await CriarCursoAsync();
			//}
			//else
			//{
			//    await CriarAlunoAsync(Guid.Parse(identityUser.Id), nome, email, dataNascimento);
			//}
			//#endregion
		}
	}
}