using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MBA.Pagamentos.Api.MigrationHelper;

[ExcludeFromCodeCoverage]
public static class SqlitePathResolver
{
	private const string DatabaseFileName = "PagamentoDB.db";
	private const string DataFolderName = "Data";

	/// <summary>
	/// Resolves the SQLite database folder path based on configuration priority:
	/// 1. Explicit SqliteFolderPath from appsettings
	/// 2. Docker container detected via DOTNET_RUNNING_IN_CONTAINER -> /app/data/
	/// 3. Debug mode -> project directory + Data/
	/// </summary>
	public static string ResolveDatabaseFolder(string? configuredFolderPath, ILogger logger)
	{
		if (!string.IsNullOrWhiteSpace(configuredFolderPath))
		{
			logger.LogInformation(
				"[SqlitePathResolver] Usando caminho configurado em SqliteFolderPath: {FolderPath}",
				configuredFolderPath);
			return configuredFolderPath;
		}

		if (Debugger.IsAttached || IsDebugBuild())
		{
			var projectDir = FindProjectDirectory();
			var debugPath = Path.Combine(projectDir, DataFolderName);
			logger.LogInformation(
				"[SqlitePathResolver] Modo Debug detectado. Usando diretório do projeto: '{FolderPath}'",
				debugPath);
			return debugPath;
		}

		var fallbackPath = Path.Combine(AppContext.BaseDirectory, DataFolderName);
		logger.LogInformation(
			"[SqlitePathResolver] Usando fallback (BaseDirectory): '{FolderPath}'",
			fallbackPath);

		return fallbackPath;
	}

	/// <summary>
	/// Builds the full SQLite connection string from a resolved folder path.
	/// Also ensures the target directory exists.
	/// </summary>
	public static string BuildConnectionString(string folderPath, ILogger logger)
	{
		EnsureDirectoryExists(folderPath, logger);

		var fullPath = Path.Combine(folderPath, DatabaseFileName);
		var connectionString = $"Data Source={fullPath}";

		logger.LogInformation(
			"[SqlitePathResolver] Connection string resolvida: '{ConnectionString}'",
			connectionString);

		return connectionString;
	}

	private static void EnsureDirectoryExists(string folderPath, ILogger logger)
	{
		if (Directory.Exists(folderPath))
			return;

		logger.LogWarning(
			"[SqlitePathResolver] Diretório nao existe. Criando: '{FolderPath}'",
			folderPath);

		Directory.CreateDirectory(folderPath);
	}

	private static string FindProjectDirectory()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);

		while (directory is not null)
		{
			if (directory.GetFiles("MBA.Pagamentos.Api.csproj").Length > 0)
				return directory.FullName;

			directory = directory.Parent;
		}

		return AppContext.BaseDirectory;
	}

	private static bool IsDebugBuild()
	{
#if DEBUG
		return true;
#else
        return false;
#endif
	}
}