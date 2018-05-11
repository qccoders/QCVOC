namespace QCVOC.Server.Data.Model.Security
{
    using System;

    public class RefreshToken
    {
        #region Public Properties

        public Guid accountid { get; set; }
        public DateTime expires { get; set; }
        public Guid id { get; set; }
        public DateTime issued { get; set; }

        #endregion Public Properties
    }
}