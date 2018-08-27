using System.Web;
using SimpleMultiTenancy.Data.Abstract;

namespace SimpleMultiTenancy.Infrastructure
{
    public class TenantCodeResolver : ITenantCodeResolver
    {
        public string GetTenantCode()
        {
            var httpContextBase = HttpContext.Current;
            var tenant = DemoHelper.TryGetTentantCode(httpContextBase);
            return tenant;
        }
    }
}