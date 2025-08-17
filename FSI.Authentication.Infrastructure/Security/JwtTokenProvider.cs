using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FSI.Authentication.Domain.Aggregates;
using Microsoft.IdentityModel.Tokens;
using AppServ = FSI.Authentication.Application.Interfaces.Services;

namespace FSI.Authentication.Infrastructure.Security
{
    public sealed class JwtTokenProvider : AppServ.ITokenProvider
    {
        private readonly JwtOptions _opt;
        public JwtTokenProvider(JwtOptions opt) => _opt = opt;

        public AppServ.AccessToken CreateToken(UserAccount user)
        {
            var now = DateTimeOffset.UtcNow;
            var expires = now.AddMinutes(_opt.ExpirationMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,   user.UserId.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("profile", user.ProfileName),
                // se tiver permissões/roles, adicione aqui:
                // new("perm", "users.read"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: expires.UtcDateTime,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new AppServ.AccessToken(tokenString, expires);
        }
    }
}
