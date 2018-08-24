using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcDemo.MultiTenancy
{
    public interface ITenant
    {
        string Alias { get; }
    }

    public interface ITenantService
    {
        IList<ITenant> GetAllTenants();
    }

    public interface ITenantContextService
    {
        ITenant GetCurrentTenant();
    }

    #region impl : HostnameTenant

    public class HostnameTenant : ITenant
    {
        public HostnameTenant()
        {
            Hostnames = new List<string>();
        }

        public string Alias { get; set; }

        public IList<string> Hostnames { get; set; }


        public static HostnameTenant Empty = new HostnameTenant() { Alias = "Unkown" };
    }

    public interface IHostnameTenantService : ITenantService
    {
        new IList<HostnameTenant> GetAllTenants();
    }

    public class TenantContextService : ITenantContextService
    {
        private readonly IHostnameTenantService _tenantService;

        public TenantContextService(IHostnameTenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public ITenant GetCurrentTenant()
        {
            var allTenants = _tenantService.GetAllTenants();
            var hostname = GetHostname(HttpContext.Current);
            var tenant = allTenants.SingleOrDefault(x => x.Hostnames.Contains(hostname));
            if (tenant == null)
            {
                return HostnameTenant.Empty;
            }
            return tenant;
        }

        public string GetHostname(HttpContext context)
        {
            var hostname = context.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped).ToLowerInvariant();
            return hostname;
        }
    }

    #endregion
}
