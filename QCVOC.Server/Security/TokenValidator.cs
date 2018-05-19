namespace QCVOC.Server.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;

    public class TokenValidator : ITokenValidator
    {
        #region Public Constructors

        public TokenValidator(TokenValidationParameters tokenValidationParameters)
        {
            TokenValidationParameters = tokenValidationParameters;
        }

        #endregion Public Constructors

        #region Public Methods

        public bool TryParseAndValidateToken(string token, out JwtSecurityToken jwtSecurityToken)
        {
            jwtSecurityToken = default(JwtSecurityToken);

            try
            {
                SecurityToken securityToken;
                new JwtSecurityTokenHandler().ValidateToken(token, TokenValidationParameters, out securityToken);

                jwtSecurityToken = new JwtSecurityToken(token);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Public Methods

        #region Private Properties

        private TokenValidationParameters TokenValidationParameters { get; set; }

        #endregion Private Properties
    }
}