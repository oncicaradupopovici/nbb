using Microsoft.AspNetCore.Http;
using NBB.Contracts.Domain.ServicesContracts;

namespace NBB.Contracts.Api
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpAccessor;

        public TenantService(IHttpContextAccessor httpAccessor)
        {
            _httpAccessor = httpAccessor;
        }

        public string GetTenantId()
            => _httpAccessor.HttpContext.Request.Headers["TenantId"].ToString();
    }
}