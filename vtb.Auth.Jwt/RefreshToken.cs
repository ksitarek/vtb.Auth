using System;

namespace vtb.Auth.Jwt
{
    public class RefreshToken
    {
        public RefreshToken(string token, DateTime created, DateTime expires)
        {
            Token = token;
            Created = created;
            Expires = expires;
        }

        public string Token { get; }
        public DateTime Created { get; }
        public DateTime Expires { get; }
    }
}