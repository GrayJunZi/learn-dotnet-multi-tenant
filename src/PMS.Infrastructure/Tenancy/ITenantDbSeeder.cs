namespace PMS.Infrastructure.Tenancy;

public interface ITenantDbSeeder
{
    Task InitializeDatabaseAsync(CancellationToken cancellationToken);
}
