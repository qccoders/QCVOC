namespace QCVOC.Server.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;
    using QCVOC.Server.Data.Model.Security;
    using Utility = Utility;

    public class JwtFactory : IJwtFactory
    {
        #region Public Methods

        public Jwt GetJwt(Account account)
        {
            return GetJwt(account, Guid.NewGuid());
        }

        public Jwt GetJwt(Account account, Guid refreshTokenId)
        {
            return new Jwt()
            {
                Account = account,
                AccessJwtSecurityToken = GetAccessToken(account, refreshTokenId),
                RefreshJwtSecurityToken = GetRefreshToken(refreshTokenId)
            };
        }

        #endregion Public Methods

        #region Private Methods

        private JwtSecurityToken GetAccessToken(Account account, Guid refreshTokenId)
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

            return GetJwtSecurityToken(expiry, claims);
        }

        private JwtSecurityToken GetJwtSecurityToken(int ttlInMinutes, params Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Utility.GetSetting<string>(Settings.JwtKey)));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: Utility.GetSetting<string>(Settings.JwtIssuer),
                audience: Utility.GetSetting<string>(Settings.JwtAudience),
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(ttlInMinutes),
                signingCredentials: credentials
            );

            return token;
        }

        private JwtSecurityToken GetRefreshToken(Guid id)
        {
            var expiry = Utility.GetSetting<int>(Settings.JwtRefreshTokenExpiry);

            var claims = new Claim[]
            {
                new Claim("jti", id.ToString())
            };

            return GetJwtSecurityToken(expiry, claims);
        }

        #endregion Private Methods
    }
}