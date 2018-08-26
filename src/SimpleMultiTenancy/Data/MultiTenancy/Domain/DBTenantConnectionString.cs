using SimpleMultiTenancy.Data.Abstract;
using System;

namespace SimpleMultiTenancy.Data.Domain
{
    public class DBTenantConnectionString : IDBTenantConnectionString
    {
        public Guid TenantID { get; set; }
        public string ConnString { get; set; }
    }
}