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
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                return builder.UseNpgsql(connectionString,
                    e => e.MigrationsAssembly("CleanArchitecture.Blazor.Migrators.PostgreSQL"))
                    .UseSnakeCaseNamingConvention();
            case DbProviderKeys.SqlServer:
                return builder.UseSqlServer(connectionString,
                    e => e.MigrationsAssembly("CleanArchitecture.Blazor.Migrators.MSSQL"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);//verify this necessary
            case DbProviderKeys.SqLite:
                return builder.UseSqlite(connectionString,
                    e => e.MigrationsAssembly("CleanArchitecture.Blazor.Migrators.SqLite"));
            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }
}