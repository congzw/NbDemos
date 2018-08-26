using SimpleMultiTenancy.Data.Abstract;
using System;

namespace SimpleMultiTenancy.Data.Domain
{
    public class Tenant : Entity, ITenant
    {
        public Tenant()
        {
            CreatedDate = DateTime.UtcNow;
            TenantID = Guid.NewGuid();
        }

        public Guid TenantID { get; set; }

        public string TenantName { get; set; }

        public string TenantCode { get; set; }

        public string ApplicationTitle { get; set; }
    }
}