namespace QCVOC.Server.Data.DTO
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Newtonsoft.Json;
    using QCVOC.Server.Data.Model.Security;

    public class TokenResponse
    {
        #region Public Constructors

        public TokenResponse(JwtSecurityToken accessJwtSecurityToken, JwtSecurityToken refreshJwtSecurityToken)
        {
            AccessJwtSecurityToken = accessJwtSecurityToken;
            RefreshJwtSecurityToken = refreshJwtSecurityToken;
        }

        #endregion Public Constructors

        #region Public Properties

        [JsonIgnore]
        public JwtSecurityToken AccessJwtSecurityToken { get; set; }

        public string AccessToken => new JwtSecurityTokenHandler().WriteToken(AccessJwtSecurityToken);

        [JsonIgnore]
        public Account Account { get; set; }

        public DateTime Expires => AccessJwtSecurityToken.ValidTo;
        public DateTime Issued => AccessJwtSecurityToken.ValidFrom;

        [JsonIgnore]
        public JwtSecurityToken RefreshJwtSecurityToken { get; set; }

        public string RefreshToken => new JwtSecurityTokenHandler().WriteToken(RefreshJwtSecurityToken);

        [JsonIgnore]
        public Guid RefreshTokenId => Guid.Parse(RefreshJwtSecurityToken.Id);

        public string TokenType => JwtBearerDefaults.AuthenticationScheme;

        #endregion Public Properties
    }
}