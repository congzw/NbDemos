using System;

namespace SimpleMultiTenancy.Data.Abstract
{
    public interface ITenant
    {
        Guid TenantID { get; set; }

        string TenantName { get; set; }

        string TenantCode { get; set; }

        string ApplicationTitle { get; set; }

        //string Domains { get; set; }
    }
}