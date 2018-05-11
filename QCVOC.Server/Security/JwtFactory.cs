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
                RefreshTokenId = refreshTokenId,
                AccessJwtSecurityToken = GetAccessToken(account),
                RefreshJwtSecurityToken = GetRefreshToken(refreshTokenId)
            };
        }

        public bool TryParseJwtSecurityToken(string token, out JwtSecurityToken jwtSecurityToken)
        {
            jwtSecurityToken = default(JwtSecurityToken);
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.Can;

                jwtSecurityToken = new JwtSecurityToken(token);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private JwtSecurityToken GetAccessToken(Account account)
        {
            var expiry = Utility.GetSetting<int>(Settings.JwtAccessTokenExpiry, Constants.JwtAccessTokenExpiryDefault);

            var claims = new Claim[]
            {
                new Claim("id", account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Name),
                new Claim(ClaimTypes.Role, account.Role.ToString()),
            };

            return GetJwtSecurityToken(expiry, claims);
        }

        private JwtSecurityToken GetJwtSecurityToken(int expires, params Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Utility.GetSetting<string>("JwtKey", Constants.JwtKeyDefault)));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: Utility.GetSetting<string>(Settings.JwtIssuer, Constants.JwtIssuerDefault),
                audience: Utility.GetSetting<string>(Settings.JwtAudience, Constants.JwtAudienceDefault),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expires),
                signingCredentials: credentials
            );

            return token;
        }

        private JwtSecurityToken GetRefreshToken(Guid id)
        {
            var expiry = Utility.GetSetting<int>(Settings.JwtRefreshTokenExpiry, Constants.JwtRefreshTokenExpiryDefault);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Hash, id.ToString())
            };

            return GetJwtSecurityToken(expiry, claims);
        }

        #endregion Private Methods
    }
}