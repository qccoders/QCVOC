namespace QCVOC.Server.Data.DTO
{
    using System.IdentityModel.Tokens.Jwt;

    public class JwtResponse
    {
        #region Private Properties

        public JwtResponse(JwtSecurityToken accessToken, JwtSecurityToken refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public JwtSecurityToken AccessToken { get; }
        public JwtSecurityToken RefreshToken { get; }

        #endregion Private Properties
    }
}