using SimpleMultiTenancy.Data.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using SimpleMultiTenancy.Infrastructure;

namespace SimpleMultiTenancy.Data
{
    public class TenantContext : DbContext
    {
        public TenantContext() : base(ContextHelper.DefaultConn)
        {
            Database.SetInitializer<TenantContext>(new TenantDBInitializer()); // Create Database If Not Exists
        }

        public DbSet<Tenant> Tenants { get; set; }
        
        public DbSet<DBTenantConnectionString> DBTenantConnectionStrings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Tenant model builder

            modelBuilder.Entity<Tenant>()
                .HasKey(c => c.TenantID);

            modelBuilder.Entity<Tenant>()
                .Property(c => c.TenantName)
                .IsRequired()
                .HasMaxLength(128);

            modelBuilder.Entity<Tenant>()
                .Property(c => c.TenantCode)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));

            modelBuilder.Entity<Tenant>()
                .Property(c => c.ApplicationTitle)
                .IsRequired()
                .HasMaxLength(128);

            modelBuilder.Entity<Tenant>()
                .Property(c => c.CreatedBy).IsRequired().HasMaxLength(128);

            modelBuilder.Entity<Tenant>()
                .Property(c => c.UpdatedBy).HasMaxLength(128);

            #endregion Tenant model builder
            
            #region Tenant DB Configuration
            modelBuilder.Entity<DBTenantConnectionString>()
                .HasKey(c => c.TenantID);

            modelBuilder.Entity<DBTenantConnectionString>()
                .Property(c => c.ConnString)
                .IsRequired()
                .HasMaxLength(256);
            #endregion
        }
    }
}