using System.Linq;
using System.Web;
using SimpleMultiTenancy.Data;
using SimpleMultiTenancy.Data.Demos;

namespace SimpleMultiTenancy.Infrastructure
{
    public class ContextHelper
    {
        public static TenantContext CreateTenantContext()
        {
            var tenantDbFactory = new TenantContextFactory();
            var tenantContext = tenantDbFactory.Get();
            return tenantContext;
        }

        public static DemoContext CreateDemoContext()
        {
            var tenantDbFactory = new TenantContextFactory();
            var tenantResolver = new TenantResolver(tenantDbFactory, HttpContext.Current);

            var currentTenant = tenantResolver.GetCurrentTenant;
            if (currentTenant == null)
            {
                return null;
            }
            var tenantDbConnectionString = tenantResolver.GetTenantDBConnectionString;
            return new DemoContext(tenantDbConnectionString.ConnString);
        }

        public const string DefaultConn = "DefaultConnection";
        public static void SetInitializers(bool dropDbFirst, string defaultConn)
        {
            if (dropDbFirst)
            {
                System.Data.Entity.Database.Delete(defaultConn);
            }
            System.Data.Entity.Database.SetInitializer(new TenantDBInitializer());

            var tenantContext = ContextHelper.CreateTenantContext();
            var tenants = tenantContext.Tenants.OrderBy(x => x.TenantCode).ToList();
            var dbTenantConnectionStrings = tenantContext.DBTenantConnectionStrings.ToList();
            foreach (var tenant in tenants)
            {
                var dbTenantConnectionString = dbTenantConnectionStrings.SingleOrDefault(x => x.TenantID == tenant.TenantID);
                if (dbTenantConnectionString != null)
                {
                    if (dropDbFirst)
                    {
                        System.Data.Entity.Database.Delete(dbTenantConnectionString.ConnString);
                    }
                    System.Data.Entity.Database.SetInitializer(new DemoContextInitializer(tenant.TenantCode));
                }
            }
        }
    }
}
