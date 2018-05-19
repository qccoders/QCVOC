namespace QCVOC.Server.Data.Model.Security
{
    using Dapper.Contrib.Extensions;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class RefreshToken
    {
        #region Public Properties

        [ExplicitKey]
        public Guid AccountId { get; set; }

        [Column("expires")]
        public DateTime Expires { get; set; }

        [Column("issued")]
        public DateTime Issued { get; set; }

        [Column("tokenid")]
        public Guid TokenId { get; set; }

        #endregion Public Properties
    }
}