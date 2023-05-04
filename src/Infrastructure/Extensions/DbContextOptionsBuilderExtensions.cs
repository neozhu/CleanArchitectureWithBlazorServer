using CleanArchitecture.Blazor.Infrastructure.Constants.Database;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

internal static class DbContextOptionsBuilderExtensions
{
    internal static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider,
        string connectionString)
    {
        switch (dbProvider.ToLowerInvariant())
        {
            case DbProviderKeys.Npgsql:
                return builder.UseNpgsql(connectionString,
                    e =>
                    {
                        e.MigrationsAssembly("CleanArchitecture.Blazor.Migrators.PostgreSQL");
                        e.UseNodaTime();
                    });
            case DbProviderKeys.SqlServer:
                return builder.UseSqlServer(connectionString,
                    e =>
                    {
                        e.MigrationsAssembly("CleanArchitecture.Blazor.Migrators.MSSQL");
                        e.UseNodaTime();
                    });
            case DbProviderKeys.SqLite:
                return builder.UseSqlite(connectionString,
                    e => e.MigrationsAssembly("CleanArchitecture.Blazor.Migrators.SqLite"));
            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }
}