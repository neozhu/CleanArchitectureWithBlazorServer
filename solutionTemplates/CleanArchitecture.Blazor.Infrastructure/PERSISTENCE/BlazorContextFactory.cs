namespace CleanArchitecture.Blazor.$safeprojectname$.Persistence;
public class BlazorContextFactory<TContext> : IDbContextFactory<TContext> where TContext : DbContext
{
    private readonly IServiceProvider provider;

    public BlazorContextFactory(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public TContext CreateDbContext()
    {
        if (provider == null)
        {
            throw new InvalidOperationException(
                $"You must configure an instance of IServiceProvider");
        }

        return ActivatorUtilities.CreateInstance<TContext>(provider);
    }
}
