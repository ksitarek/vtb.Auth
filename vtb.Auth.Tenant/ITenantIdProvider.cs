using System;

namespace vtb.Auth.Tenant
{
    public interface ITenantIdProvider
    {
        Guid TenantId { get; set; }
    }
}