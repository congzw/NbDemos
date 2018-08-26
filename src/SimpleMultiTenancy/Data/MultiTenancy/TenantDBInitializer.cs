using SimpleMultiTenancy.Data.Domain;
using System;
using System.Data.Entity;

namespace SimpleMultiTenancy.Data
{
    public class TenantDBInitializer : DropCreateDatabaseIfModelChanges<TenantContext>
    {
        protected override void Seed(TenantContext context)
        {
            //Create dummy tenant
            for (int i = 0; i < 2; i++)
            {
                var tenant = new Tenant()
                {
                    ApplicationTitle = string.Format("Tenant {0} App", i.ToString("00")),
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow,
                    TenantCode = string.Format("CMP{0}", i.ToString("00")),
                    TenantName = string.Format("Company {0}", i.ToString("00"))
                };

                var database = context.Database.Connection.Database;
                var tenantDbName = string.Format("{0}_{1}", database, tenant.TenantCode);
                var tenantConnectionConnString =
                    string.Format(
                        "data source=.;initial catalog={0};Integrated Security=true;multipleactiveresultsets=True;application name=EntityFramework",
                        tenantDbName);
                //System.Data.Entity.Database.Delete(tenantConnectionConnString);

                var dbSetting = new DBTenantConnectionString()
                {
                    TenantID = tenant.TenantID,
                    //mock for temp
                    ConnString = tenantConnectionConnString
                };

                context.Tenants.Add(tenant);
                context.DBTenantConnectionStrings.Add(dbSetting);
            }

            context.SaveChanges();

            base.Seed(context);
        }
    }
}