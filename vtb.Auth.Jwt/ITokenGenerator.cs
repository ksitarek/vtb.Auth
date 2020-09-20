using System;

namespace vtb.Auth.Jwt
{
    public interface ITokenGenerator
    {
        Jwt GetJwt(Guid userId, Guid tenantId, string[] roles, string[] permissions);

        Jwt GetSystemJwt(Guid tenantId, string traceIdentifier);

        RefreshToken GetRefreshToken();
    }
}