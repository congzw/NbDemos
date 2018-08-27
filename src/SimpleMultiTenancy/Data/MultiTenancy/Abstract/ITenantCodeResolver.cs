namespace SimpleMultiTenancy.Data.Abstract
{
    public interface ITenantCodeResolver
    {
        string GetTenantCode();
    }
}