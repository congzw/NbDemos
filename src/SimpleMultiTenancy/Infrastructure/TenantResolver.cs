using SimpleMultiTenancy.Data;
using SimpleMultiTenancy.Data.Abstract;
using System.Linq;
using SimpleMultiTenancy.Data.MultiTenancy.Abstract;

namespace SimpleMultiTenancy.Infrastructure
{
    public class TenantResolver : ITenantResolver
    {
        private readonly ITenantCodeResolver _tenantCodeResolver;
        private readonly TenantContext tenantContext;

        public TenantResolver(
            ITenantContextFactory tenantContextFactory,
            ITenantCodeResolver tenantCodeResolver)
        {
            _tenantCodeResolver = tenantCodeResolver;
            tenantContext = tenantContextFactory.Get();
        }

        private ITenant tenantData = null;

        public ITenant GetCurrentTenant
        {
            get
            {
                if (tenantData == null)
                {
                    var tenant = _tenantCodeResolver.GetTenantCode();
                    tenantData = tenantContext.Tenants.FirstOrDefault(x => x.TenantCode == tenant);
                }
                return tenantData;
            }
        }

        private IDBTenantConnectionString tenantConnectionString = null;

        public IDBTenantConnectionString GetTenantDBConnectionString
        {
            get
            {
                var tenant =  this.GetCurrentTenant;
                if (tenant != null)
                {
                    if (tenantConnectionString == null)
                    {
                        tenantConnectionString = tenantContext.DBTenantConnectionStrings.FirstOrDefault(x => x.TenantID == tenant.TenantID);
                    }
                    return tenantConnectionString;
                }
                return tenantConnectionString;
            }
        }
    }
}