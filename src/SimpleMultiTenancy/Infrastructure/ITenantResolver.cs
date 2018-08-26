using SimpleMultiTenancy.Data.Abstract;

namespace SimpleMultiTenancy.Infrastructure
{
    public interface ITenantResolver
    {
        ITenant GetCurrentTenant { get; }

        IDBTenantConnectionString GetTenantDBConnectionString { get; }
    }
}