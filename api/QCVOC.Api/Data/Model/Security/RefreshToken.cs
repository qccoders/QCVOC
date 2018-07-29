namespace QCVOC.Api.Data.Model.Security
{
    using Dapper.Contrib.Extensions;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class RefreshToken : IEquatable<RefreshToken>
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

        public bool Equals(RefreshToken token)
        {
            if(token == null)
                return this == null;
            
            return this.TokenId == token.TokenId
            && this.AccountId == token.AccountId
            && (this.Expires - token.Expires <= TimeSpan.FromSeconds(1))
            && (this.Issued - token.Issued <= TimeSpan.FromSeconds(1));
        }
    }
}