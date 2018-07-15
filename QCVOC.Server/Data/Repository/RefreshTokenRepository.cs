using System.Data;
using QCVOC.Server.Data.ConnectionFactory;
using QCVOC.Server.Data.Model.Security;
using Dapper.Contrib.Extensions;
using Dapper;
using System;
using System.Text;
using System.Collections.Generic;

namespace QCVOC.Server.Data.Repository
{
    /// <summary>
    /// </summary>
    /// <typeparam name="RefreshToken"></typeparam>
    public class RefreshTokenRepository : IRepository<RefreshToken>
    {
        #region Public Constructors

        public RefreshTokenRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        #endregion Public Constructors

        #region Private Properties

        private IDbConnectionFactory ConnectionFactory { get; }

        #endregion Private Properties

        #region Public Methods

        public RefreshToken Create(RefreshToken token)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute("INSERT INTO refreshtokens(tokenid, issued, expires, accountid) VALUES(@tokenid, @issued, @expires, @accountid);", new
                {
                    tokenid = token.TokenId,
                    issued = token.Issued,
                    expires = token.Expires,
                    accountid = token.AccountId
                });
                return Get(token.AccountId);
            }
        }

        public void Delete(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute("DELETE FROM refreshtokens WHERE accountId = @id ", new { id = id });
            }
        }

        public void Delete(RefreshToken token)
        {
            if (token == null)
                throw new ArgumentException("token cannot be null.", nameof(token));

            Delete(token.AccountId);
        }

        public RefreshToken Get(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.QueryFirstOrDefault<RefreshToken>("SELECT accountid, expires, issued, tokenid FROM refreshtokens WHERE accountid = @tokenid ", new { tokenid = id });
            }
        }

        public IEnumerable<RefreshToken> GetAll()
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<RefreshToken>("SELECT accountid, expires, issued, tokenid FROM refreshtokens;");
            }
        }

        public RefreshToken Update(RefreshToken token)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute("UPDATE refreshtokens SET expires = @expires, issued = @issued, tokenid = @tokenid where accountid = @id;", new
                {
                    id = token.AccountId,
                    expires = token.Expires,
                    issued = token.Issued,
                    tokenid = token.TokenId
                });
                return Get(token.AccountId);
            }
        }

        #endregion Public Methods
    }
}