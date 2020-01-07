using Microsoft.Extensions.Configuration;
using NBB.Tenancy.Abstractions;
using System;
using System.Collections.Generic;

namespace NBB.Tenancy.Impl
{
    public class TenantConfig : ITenantConfig
    {
        private const string TenantId1 = "Tenant_1";
        private const string TenantId2 = "Tenant_2";
        private const string TenantId3 = "Tenant_3";
        private const string TenantId4 = "Tenant_4";
        private const string TenantId5 = "Tenant_5";
        private const string TenantId6 = "Tenant_6";

        public readonly Dictionary<string, TenantType> Tenants =
        //new Dictionary<string, TenantType>()
        //{
        //    [TenantId1] = TenantType.Dedicated,
        //    [TenantId2] = TenantType.Dedicated,
        //    [TenantId3] = TenantType.Shared,
        //    [TenantId4] = TenantType.Shared,
        //    [TenantId5] = TenantType.Dedicated,
        //    [TenantId6] = TenantType.Dedicated,
        //};
        AddOneHundredTenants();

        private readonly IConfiguration _configuration;
        private static int numberOfTopics;

        public TenantConfig(IConfiguration configuration)
        {
            _configuration = configuration;
            numberOfTopics = Convert.ToInt32(_configuration.GetSection("Messaging")["NumberOfTopics"]);
        }

        public TenantType GetTenantType(string tenantId)
            => tenantId == null ? TenantType.None : Tenants.TryGetValue(tenantId, out var value) ? value : TenantType.None;

        public IEnumerable<string> GetTenantIds()
            => new List<string> { TenantId1, TenantId2, TenantId3, TenantId4, TenantId5, TenantId6 };

        public IEnumerable<string> GetOneHundredTenants()
        {
            var list = new List<string>();
            for (int i = 1; i < 1000; i++)
            {
                list.Add($"Tenant_{i}");
            }
            return list;
        }

        private static Dictionary<string, TenantType> AddOneHundredTenants()
        {
            Dictionary<string, TenantType> Tenants = new Dictionary<string, TenantType>();

            for (int i = 1; i < 1000; i++)
            {
                Tenants.Add($"Tenant_{i}", TenantType.Dedicated);
            }

            return Tenants;
        }
    }
}