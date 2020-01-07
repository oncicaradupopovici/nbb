using System.Collections.Generic;

namespace NBB.Tenancy.Abstractions
{
    public interface ITenantConfig
    {
        IEnumerable<string> GetTenantIds();
        IEnumerable<string> GetOneHundredTenants();
        TenantType GetTenantType(string tenantId);
    }
}