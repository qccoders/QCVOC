namespace QCVOC.Server
{
    public static class Constants
    {
        #region Public Fields

        public static readonly int JwtAccessTokenExpiryDefault = 30;
        public static readonly string JwtAudienceDefault = "QCVOC";
        public static readonly string JwtIssuerDefault = "QCVOC";
        public static readonly string JwtKeyDefault = "EE26B0DD4AF7E749AA1A8EE3C10AE9923F618980772E473F8819A5D4940E0DB27AC185F8A0E1D5F84F88BC887FD67B143732C304CC5FA9AD8E6F57F50028A8FF";
        public static readonly int JwtRefreshTokenExpiryDefault = 3600;

        #endregion Public Fields
    }
}