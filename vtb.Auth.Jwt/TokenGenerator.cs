using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace vtb.Auth.Jwt
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IOptions<JwtSettings> _jwtOptions;
        private readonly ISystemClock _systemClock;

        public TokenGenerator(ISystemClock systemClock, IOptions<JwtSettings> jwtOptions)
        {
            _systemClock = systemClock;
            _jwtOptions = jwtOptions;
        }

        private byte[] Key => Encoding.ASCII.GetBytes(_jwtOptions.Value.Secret);
        private SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Key);

        private SigningCredentials SigningCredentials =>
            new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

        public Jwt GetJwt(Guid userId, Guid tenantId, string[] roles, string[] permissions)
        {
            var claims = new List<Claim>
            {
                new Claim("user_id", userId.ToString()),
                new Claim("tenant_id", tenantId.ToString())
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            claims.AddRange(permissions.Select(p => new Claim("permission", p)));

            return DoGenerateClaimsJwt(claims);
        }

        public Jwt GetSystemJwt(Guid tenantId, string traceIdentifier)
        {
            return DoGenerateClaimsJwt(new List<Claim>
            {
                new Claim("system", true.ToString()),
                new Claim("trace_id", traceIdentifier),
                new Claim("tenant_id", tenantId.ToString())
            });
        }

        public RefreshToken GetRefreshToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);

                var now = _systemClock.UtcNow.DateTime;
                var expires = now.Add(_jwtOptions.Value.RefreshTokenLifespan);

                return new RefreshToken(Convert.ToBase64String(randomBytes), now, expires);
            }
        }

        private Jwt DoGenerateClaimsJwt(List<Claim> claims)
        {
            var issuedAt = _systemClock.UtcNow.UtcDateTime;
            var expires = issuedAt.Add(_jwtOptions.Value.JwtTokenLifespan);

            claims.Add(new Claim("token_id", Guid.NewGuid().ToString()));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),

                Expires = expires,
                IssuedAt = issuedAt,
                NotBefore = issuedAt,
                SigningCredentials = SigningCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return new Jwt(tokenHandler.WriteToken(securityToken), expires);
        }
    }
}