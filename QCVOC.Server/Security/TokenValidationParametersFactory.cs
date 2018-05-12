namespace QCVOC.Server.Security
{
    using System.Text;
    using Microsoft.IdentityModel.Tokens;
    using Utility = Utility;

    public class TokenValidationParametersFactory
    {
        #region Public Methods

        public TokenValidationParameters GetParameters()
        {
            return new TokenValidationParameters
            {
                ValidIssuer = Utility.GetSetting<string>(Settings.JwtIssuer, Constants.JwtIssuerDefault),
                ValidateIssuer = true,
                ValidAudience = Utility.GetSetting<string>(Settings.JwtAudience, Constants.JwtAudienceDefault),
                ValidateAudience = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Utility.GetSetting<string>(Settings.JwtKey, Constants.JwtKeyDefault))),
                ValidateIssuerSigningKey = true,
            };
        }

        #endregion Public Methods
    }
}
