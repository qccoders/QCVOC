namespace QCVOC.Server
{
    public static class Settings
    {
        #region Public Fields

        public static readonly string DbConnectionString = "DbConnectionString";

        public static readonly string JwtAccessTokenExpiry = "JwtAccessTokenExpiry";
        public static readonly string JwtAudience = "JwtAudience";
        public static readonly string JwtIssuer = "JwtIssuer";
        public static readonly string JwtKey = "JwtKey";
        public static readonly string JwtRefreshTokenExpiry = "JwtRefreshTokenExpiry";

        #endregion Public Fields
    }
}