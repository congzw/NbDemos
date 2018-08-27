using SimpleMultiTenancy.Data.Abstract;

namespace SimpleMultiTenancy.Data.MultiTenancy.Abstract
{
    public interface ITenantResolver
    {
        ITenant GetCurrentTenant { get; }

        IDBTenantConnectionString GetTenantDBConnectionString { get; }
    }
}