namespace QCVOC.Server.Data.Model.Security
{
    using System;

    public class RefreshToken
    {
        #region Public Properties

        public Guid AccountId { get; set; }
        public DateTime Expires { get; set; }
        public Guid Id { get; set; }
        public DateTime Issued { get; set; }

        #endregion Public Properties
    }
}