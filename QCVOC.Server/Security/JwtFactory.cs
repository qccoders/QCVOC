using Microsoft.IdentityModel.Tokens;
using QCVOC.Server.Data.Model;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QCVOC.Server.Security
{
    public class JwtFactory
    {
        #region Public Methods

        public string GetJwt(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Utility.GetSetting<string>("JwtKey")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("name", user.Name),
                new Claim("role", user.Role.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: Utility.GetSetting<string>("JwtIssuer", "QCVOC"),
                audience: Utility.GetSetting<string>("JwtAudience", "QCVOC"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion Public Methods
    }
}