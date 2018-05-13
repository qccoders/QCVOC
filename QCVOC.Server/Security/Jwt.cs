using Newtonsoft.Json;
using QCVOC.Server.Data.Model.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QCVOC.Server.Security
{
    public class Jwt
    {
        public string AccessToken => new JwtSecurityTokenHandler().WriteToken(AccessJwtSecurityToken);
        [JsonIgnore]
        public JwtSecurityToken AccessJwtSecurityToken { get; set; }
        public string RefreshToken => new JwtSecurityTokenHandler().WriteToken(RefreshJwtSecurityToken);
        public JwtSecurityToken RefreshJwtSecurityToken { get; set; }
        public DateTime Expires => AccessJwtSecurityToken.ValidTo;
        public DateTime Issued => AccessJwtSecurityToken.ValidFrom;
        [JsonIgnore]
        public Account Account { get; set; }
        [JsonIgnore]
        public Guid RefreshTokenId => Guid.Parse(RefreshJwtSecurityToken.Claims.Where(c => c.Type == ClaimTypes.Hash).FirstOrDefault().Value);

    }
}
