using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
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
    }
}