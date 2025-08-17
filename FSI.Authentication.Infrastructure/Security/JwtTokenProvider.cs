using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using FSI.Authentication.Domain.Abstractions.Security;
using FSI.Authentication.Domain.Aggregates;

namespace FSI.Authentication.Infrastructure.Security
{
    public sealed class JwtTokenProvider : ITokenProvider
    {
        private readonly JwtOptions _opt;
        public JwtTokenProvider(JwtOptions opt) => _opt = opt;

        public string CreateAccessToken(UserAccount user)
        {
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_opt.ExpirationMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("profile", user.ProfileName),
            };
            // Se tiver permissões em outro lugar, adicione-as aqui:
            // claims.AddRange(permissions.Select(p => new Claim("perm", p)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken(UserAccount user)
            => Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + "-" + user.UserId.ToString("N");
    }
}
