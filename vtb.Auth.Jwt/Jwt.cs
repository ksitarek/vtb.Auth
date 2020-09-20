using System;

namespace vtb.Auth.Jwt
{
    public class Jwt
    {
        public Jwt(string token, DateTime expires)
        {
            Token = token;
            Expires = expires;
        }

        public string Token { get; }
        public DateTime Expires { get; }
    }
}