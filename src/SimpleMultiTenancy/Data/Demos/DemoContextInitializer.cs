using System.Data.Entity;

namespace SimpleMultiTenancy.Data.Demos
{
    public class DemoContextInitializer : DropCreateDatabaseIfModelChanges<DemoContext>
    {
        public DemoContextInitializer(string tenant)
        {
            Tenant = tenant;
        }

        public string Tenant { get; set; }

        protected override void Seed(DemoContext context)
        {
            //Create dummy tenant
            for (int i = 0; i < 3; i++)
            {
                var foo = new Foo()
                {
                    Name = string.Format("Product {0} of {1}", i.ToString("00"), Tenant)
                };
                context.Foos.Add(foo);
            }

            context.SaveChanges();
            base.Seed(context);
        }
    }
}