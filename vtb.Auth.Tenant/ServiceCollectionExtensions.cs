using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace vtb.Auth.Tenant
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTenantProvider(this IServiceCollection services)
        {
            services.AddScoped<ITenantIdProvider>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext;

                if (httpContext != null)
                {
                    var userClaims = httpContext.User.Claims;
                    var tenantIdClaim = userClaims.First(x => x.Type == "tenant_id");
                    return new ValueTenantIdProvider(Guid.Parse(tenantIdClaim.Value));
                }
                else
                {
                    return new ValueTenantIdProvider();
                }
            });

            return services;
        }
    }
}