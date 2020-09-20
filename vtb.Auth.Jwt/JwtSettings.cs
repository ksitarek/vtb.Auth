using System;

namespace vtb.Auth.Jwt
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public TimeSpan JwtTokenLifespan { get; set; } = TimeSpan.FromMinutes(15);
        public TimeSpan RefreshTokenLifespan { get; set; } = TimeSpan.FromDays(7);
    }
}