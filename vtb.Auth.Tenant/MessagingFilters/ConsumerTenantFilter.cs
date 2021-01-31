using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace vtb.Auth.Tenant.MessagingFilters
{
    public class TenantConsumeFilter<T> : IFilter<ConsumeContext<T>>
            where T : class
    {
        private readonly IServiceProvider _serviceProvider;

        public TenantConsumeFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("tenant-consume-filter");
        }

        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var tenantIdString = context.Headers.Get<string>("tenant_id", string.Empty);
            if (!string.IsNullOrEmpty(tenantIdString))
            {
                var tenantId = Guid.Parse(tenantIdString);
                var tenantIdProvider = _serviceProvider.GetService<ITenantIdProvider>();
                tenantIdProvider.TenantId = tenantId;
            }

            return next.Send(context);
        }
    }
}