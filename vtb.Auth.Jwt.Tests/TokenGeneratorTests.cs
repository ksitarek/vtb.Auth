using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace vtb.Auth.Jwt.Tests
{
    public class TokenGeneratorTests
    {
        private static readonly JwtSettings _jwtSettings = new JwtSettings
        {
            Secret = new string('_', 1000),
            JwtTokenLifespan = TimeSpan.FromMinutes(15),
            RefreshTokenLifespan = TimeSpan.FromHours(7)
        };

        private readonly DateTimeOffset _utcNow = new DateTimeOffset(2020, 06, 06, 12, 24, 21, TimeSpan.FromHours(0));

        private TokenGenerator _tokenGenerator;

        [SetUp]
        public void Setup()
        {
            var jwtOptionsMock = new Mock<IOptions<JwtSettings>>();
            jwtOptionsMock.Setup(x => x.Value).Returns(_jwtSettings);

            var systemClockMock = new Mock<ISystemClock>();
            systemClockMock.Setup(x => x.UtcNow).Returns(_utcNow);

            _tokenGenerator = new TokenGenerator(systemClockMock.Object, jwtOptionsMock.Object);
        }

        [Test]
        public void GetJwt_Returns_Jwt_With_Correct_Payload()
        {
            var userId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var roles = new[] { "Role1", "Role2" };
            var permissions = new[] { "Permission1", "Permission2" };

            var jwt = _tokenGenerator.GetJwt(userId, tenantId, roles, permissions);

            var parts = jwt.Token.Split('.');
            var payload = DecodeUrlBase64(parts[1]);
            var payloadJObject = JObject.Parse(payload);

            Assert.AreEqual(userId.ToString(), payloadJObject["user_id"].ToString());
            Assert.AreEqual(tenantId.ToString(), payloadJObject["tenant_id"].ToString());
            Assert.AreEqual("[\"Role1\",\"Role2\"]",
                payloadJObject["role"].ToString().Replace("\r", "").Replace("\n", "").Replace(" ", ""));
            Assert.AreEqual("[\"Permission1\",\"Permission2\"]",
                payloadJObject["permission"].ToString().Replace("\r", "").Replace("\n", "").Replace(" ", ""));

            Assert.AreEqual(_utcNow.ToUnixTimeSeconds(), Convert.ToUInt64(payloadJObject["iat"].ToString()));
            Assert.AreEqual(_utcNow.ToUnixTimeSeconds(), Convert.ToUInt64(payloadJObject["nbf"].ToString()));
            Assert.AreEqual(_utcNow.Add(_jwtSettings.JwtTokenLifespan).ToUnixTimeSeconds(),
                Convert.ToUInt64(payloadJObject["exp"].ToString()));
        }

        [Test]
        public void GetRefreshToken_Returns_RandomToken_With_Correct_Expiration()
        {
            var refreshToken = _tokenGenerator.GetRefreshToken();

            Assert.IsNotEmpty(refreshToken.Token);
            Assert.AreEqual(_utcNow.DateTime, refreshToken.Created);
            Assert.AreEqual(_utcNow.Add(_jwtSettings.RefreshTokenLifespan).DateTime, refreshToken.Expires);
        }

        [Test]
        public void GetRefreshToken_Returns_Different_Token_Each_Time()
        {
            var previousTokens = new HashSet<string>();

            for (var i = 0; i < 100; i++)
            {
                var refreshToken = _tokenGenerator.GetRefreshToken();

                Assert.IsNotEmpty(refreshToken.Token);
                Assert.AreEqual(_utcNow.DateTime, refreshToken.Created);
                Assert.AreEqual(_utcNow.Add(_jwtSettings.RefreshTokenLifespan).DateTime, refreshToken.Expires);

                Assert.IsFalse(previousTokens.Contains(refreshToken.Token));
                previousTokens.Add(refreshToken.Token);
            }
        }

        [Test]
        public void GetSystemJwt_Returns_Jwt_With_Correct_Payload()
        {
            var traceIdentifier = Guid.NewGuid();
            var tenantId = Guid.NewGuid();

            var jwt = _tokenGenerator.GetSystemJwt(tenantId, traceIdentifier.ToString());

            var parts = jwt.Token.Split('.');
            var payload = DecodeUrlBase64(parts[1]);
            var payloadJObject = JObject.Parse(payload);

            Assert.AreEqual(traceIdentifier.ToString(), payloadJObject["trace_id"].ToString());
            Assert.AreEqual(tenantId.ToString(), payloadJObject["tenant_id"].ToString());

            Assert.AreEqual(_utcNow.ToUnixTimeSeconds(), Convert.ToUInt64(payloadJObject["iat"].ToString()));
            Assert.AreEqual(_utcNow.ToUnixTimeSeconds(), Convert.ToUInt64(payloadJObject["nbf"].ToString()));
            Assert.AreEqual(_utcNow.Add(_jwtSettings.JwtTokenLifespan).ToUnixTimeSeconds(),
                Convert.ToUInt64(payloadJObject["exp"].ToString()));
        }

        private static string DecodeUrlBase64(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/').PadRight(4 * ((s.Length + 3) / 4), '=');
            return Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }
    }
}