namespace QCVOC.Server.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using Newtonsoft.Json;
    using QCVOC.Server.Data.Model.Security;

    public class Jwt
    {
        #region Private Properties

        [JsonIgnore]
        public JwtSecurityToken AccessJwtSecurityToken { get; set; }

        public string AccessToken => new JwtSecurityTokenHandler().WriteToken(AccessJwtSecurityToken);

        [JsonIgnore]
        public Account Account { get; set; }

        public DateTime Expires => AccessJwtSecurityToken.ValidTo;

        [JsonIgnore]
        public JwtSecurityToken RefreshJwtSecurityToken { get; set; }

        public string RefreshToken => new JwtSecurityTokenHandler().WriteToken(RefreshJwtSecurityToken);

        [JsonIgnore]
        public Guid RefreshTokenId { get; set; }

        #endregion Private Properties
    }
}