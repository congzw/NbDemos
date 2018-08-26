using SimpleMultiTenancy.Data;
using SimpleMultiTenancy.Data.Abstract;
using System.Linq;
using System.Web;

namespace SimpleMultiTenancy.Infrastructure
{
    public class TenantResolver : ITenantResolver
    {
        private readonly HttpContext httpContextBase;

        private readonly TenantContext tenantContext;

        public TenantResolver(
            ITenantContextFactory tenantContextFactory,
            HttpContext httpContext)
        {
            tenantContext = tenantContextFactory.Get();
            this.httpContextBase = httpContext;
        }

        private ITenant tenantData = null;

        public ITenant GetCurrentTenant
        {
            get
            {
                if (tenantData == null)
                {
                    var tenant = DemoHelper.TryGetTentantCode(httpContextBase);
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
                        //var dbTenantConnectionString = tenantContext.DBTenantConnectionStrings.SingleOrDefault(x => x.TenantID == tenant.TenantID);
                        tenantConnectionString = tenantContext.DBTenantConnectionStrings.FirstOrDefault(x => x.TenantID == tenant.TenantID);
                    }
                    return tenantConnectionString;
                }
                return tenantConnectionString;
            }
        }
    }
}