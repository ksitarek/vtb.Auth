using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;

namespace vtb.Auth.Tenant.MessagingFilters
{
    public class TenantMessageFilter<T>
        : IFilter<SendContext<T>>, IFilter<PublishContext<T>>
        where T : class
    {
        private readonly ITenantIdProvider _tenantIdProvider;

        public TenantMessageFilter(ITenantIdProvider tenantIdProvider)
        {
            _tenantIdProvider = tenantIdProvider;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("tenant-set-header-filter");
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            if (_tenantIdProvider.TenantId != Guid.Empty)
            {
                context.Headers.Set("tenant_id", _tenantIdProvider.TenantId.ToString());
            }

            return Task.CompletedTask;
        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            if (_tenantIdProvider.TenantId != Guid.Empty)
            {
                context.Headers.Set("tenant_id", _tenantIdProvider.TenantId.ToString());
            }

            return Task.CompletedTask;
        }
    }
}