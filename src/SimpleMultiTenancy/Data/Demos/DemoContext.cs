using System.Data.Entity;

namespace SimpleMultiTenancy.Data.Demos
{
    public class DemoContext : DbContext
    {
        public DemoContext(string connString) : base(connString)
        {
        }

        public DbSet<Foo> Foos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Foo>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Foo>()
                .Property(c => c.Name);
        }
    }
}
