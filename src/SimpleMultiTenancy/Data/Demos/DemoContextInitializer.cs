using System.Data.Entity;
using SimpleMultiTenancy.Infrastructure;

namespace SimpleMultiTenancy.Data.Demos
{
    public class DemoContextInitializer : DropCreateDatabaseIfModelChanges<DemoContext>
    {
        protected override void Seed(DemoContext context)
        {
            var currentTenant = context.GetCurrentTenant();

            //Create dummy tenant
            for (int i = 0; i < 3; i++)
            {
                var foo = new Foo()
                {
                    Name = string.Format("Product {0} ({1})", i.ToString("00"), currentTenant)
                };
                context.Foos.Add(foo);
            }

            context.SaveChanges();
            base.Seed(context);
        }
    }

    public static class DemoContextTenantAwareExtensions
    {
        public static string GetCurrentTenant(this DemoContext context)
        {
            var tenantCodeResolver = new TenantCodeResolver();
            return tenantCodeResolver.GetTenantCode();
        }

        //public static ITenantResolver GetCurrentResolver(this DemoContext context)
        //{
        //    //should use di, todo
        //    var tenantDbFactory = new TenantContextFactory();
        //    var tenantResolver = new TenantResolver(tenantDbFactory, new TenantCodeResolver());
        //    return tenantResolver;
        //}
    }
}