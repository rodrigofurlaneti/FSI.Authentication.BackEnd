using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Infrastructure.Security
{
    public sealed class JwtTokenProvider : ITokenProvider
    {
        private readonly JwtOptions _opt;
        public JwtTokenProvider(JwtOptions opt) => _opt = opt;

        public (string accessToken, DateTime expiresAtUtc) CreateAccessToken(
            Guid userId, string email, string profile, IEnumerable<string> permissions)
        {
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_opt.ExpirationMinutes);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new("profile", profile),
        };
            claims.AddRange(permissions.Select(p => new Claim("perm", p)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return (jwt, expires);
        }

        public string CreateRefreshToken(Guid userId)
            => Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + "-" + userId.ToString("N");
    }
}
