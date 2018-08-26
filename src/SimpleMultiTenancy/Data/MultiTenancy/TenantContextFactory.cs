using System;

namespace SimpleMultiTenancy.Data
{
    public interface ITenantContextFactory : IDbFactory<TenantContext>
    {
    }

    public class TenantContextFactory : IDisposable, ITenantContextFactory
    {
        private TenantContext dataContext;

        public void Dispose()
        {
            dataContext.Dispose();
        }

        public TenantContext Get()
        {
            return dataContext ?? (dataContext = new TenantContext());
        }
    }
}