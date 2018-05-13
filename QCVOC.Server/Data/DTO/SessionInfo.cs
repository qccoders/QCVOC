namespace QCVOC.Data.DTO
{
    public class SessionInfo
    {
        #region Public Properties

        public SessionInfoCredentials Credentials { get; set; }

        public string RefreshToken { get; set; }

        #endregion Public Properties
    }
}