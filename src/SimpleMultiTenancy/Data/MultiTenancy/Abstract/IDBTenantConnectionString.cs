using System;

namespace SimpleMultiTenancy.Data.Abstract
{
    public interface IDBTenantConnectionString
    {
        Guid TenantID { get; set; }

        string ConnString { get; set; }
    }
}