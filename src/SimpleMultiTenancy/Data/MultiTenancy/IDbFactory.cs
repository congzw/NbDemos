using System;
using System.Data.Entity;

namespace SimpleMultiTenancy.Data
{
    public interface IDbFactory<out TContext> : IDisposable
        where TContext : DbContext
    {
        TContext Get();
    }
}
