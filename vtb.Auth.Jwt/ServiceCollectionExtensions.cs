using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text;
using vtb.Utils;

namespace vtb.Auth.Jwt
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, string secret)
        {
            Check.NotEmpty(secret, nameof(secret));

            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = "Bearer";
                x.DefaultChallengeScheme = "Bearer";
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,

                        LifetimeValidator = (notBefore, expires, securityToken, tokenValidationParameters) =>
                        {
                            var utcNow = DateTime.UtcNow;
                            return notBefore <= utcNow
                                   && expires > utcNow;
                        }
                    };
                });

            return services;
        }

        public static IServiceCollection AddUserProvider(this IServiceCollection services)
        {
            services.AddScoped<IUserIdProvider>(sp => {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext;

                if (httpContext != null && httpContext.User.Identity.IsAuthenticated)
                {
                    var userClaims = httpContext.User.Claims;
                    var userIdClaim = userClaims.First(x => x.Type == "user_id");

                    return new ValueUserIdProvider(Guid.Parse(userIdClaim.Value));
                }
                else
                {
                    return new ValueUserIdProvider();
                }
            });

            return services;
        }
    }
}