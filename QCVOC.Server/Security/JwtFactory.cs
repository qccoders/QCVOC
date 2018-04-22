using Microsoft.IdentityModel.Tokens;
using QCVOC.Server.Data.Model;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QCVOC.Server.Security
{
    public class JwtFactory : IJwtFactory
    {
        #region Public Methods

        public JwtSecurityToken GetJwt(Account account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Utility.GetSetting<string>("JwtKey", "EE26B0DD4AF7E749AA1A8EE3C10AE9923F618980772E473F8819A5D4940E0DB27AC185F8A0E1D5F84F88BC887FD67B143732C304CC5FA9AD8E6F57F50028A8FF")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var claims = new[]
            {
                new Claim("name", account.Name),
                new Claim("role", account.Role.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: Utility.GetSetting<string>("JwtIssuer", "QCVOC"),
                audience: Utility.GetSetting<string>("JwtAudience", "QCVOC"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return token;
        }

        #endregion Public Methods
    }
}