namespace QCVOC.Server.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;
    using QCVOC.Server.Data.Model.Security;
    using Utility = Utility;

    public class TokenFactory : ITokenFactory
    {
        #region Public Methods

        public JwtSecurityToken GetAccessToken(Account account, JwtSecurityToken refreshToken)
        {
            var jti = refreshToken.Claims.Where(c => c.Type == "jti").FirstOrDefault().Value;
            return GetAccessToken(account, Guid.Parse(jti));
        }

        public JwtSecurityToken GetAccessToken(Account account, Guid refreshTokenId)
        {
            var expiry = Utility.GetSetting<int>(Settings.JwtAccessTokenExpiry);
            var key = Utility.GetSetting<string>(Settings.JwtKey);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, account.Name),
                new Claim(ClaimTypes.Role, account.Role.ToString()),
                new Claim("sub", account.Id.ToString()),
                new Claim("name", account.Name),
                new Claim("role", account.Role.ToString()),
                new Claim("jti", refreshTokenId.ToString())
            };

            return GetJwtSecurityToken(claims, expiry);
        }

        public JwtSecurityToken GetRefreshToken()
        {
            var expiry = Utility.GetSetting<int>(Settings.JwtRefreshTokenExpiry);
            return GetRefreshToken(Guid.NewGuid(), DateTime.UtcNow.AddMinutes(expiry));
        }

        public JwtSecurityToken GetRefreshToken(Guid refreshTokenId)
        {
            var expiry = Utility.GetSetting<int>(Settings.JwtRefreshTokenExpiry);
            return GetRefreshToken(refreshTokenId, DateTime.UtcNow.AddMinutes(expiry));
        }

        public JwtSecurityToken GetRefreshToken(Guid refreshTokenId, int ttlInMinutes)
        {
            return GetRefreshToken(refreshTokenId, DateTime.UtcNow.AddMinutes(ttlInMinutes));
        }

        public JwtSecurityToken GetRefreshToken(Guid refreshTokenId, DateTime expiry)
        {
            var claims = new Claim[]
            {
                new Claim("jti", refreshTokenId.ToString())
            };

            return GetJwtSecurityToken(claims, expiry);
        }

        #endregion Public Methods

        #region Private Methods

        private JwtSecurityToken GetJwtSecurityToken(Claim[] claims, int ttlInMinuntes)
        {
            return GetJwtSecurityToken(claims, DateTime.UtcNow.AddMinutes(ttlInMinuntes));
        }

        private JwtSecurityToken GetJwtSecurityToken(Claim[] claims, DateTime expiresUtc)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Utility.GetSetting<string>(Settings.JwtKey)));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: Utility.GetSetting<string>(Settings.JwtIssuer),
                audience: Utility.GetSetting<string>(Settings.JwtAudience),
                claims: claims,
                notBefore: DateTime.Now,
                expires: expiresUtc,
                signingCredentials: credentials
            );

            return token;
        }

        #endregion Private Methods
    }
}