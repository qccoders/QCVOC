using System.Data;
using QCVOC.Server.Data.ConnectionFactory;
using QCVOC.Server.Data.Model.Security;
using Dapper.Contrib.Extensions;
using Dapper;
using System;
using System.Text;

namespace QCVOC.Server.Data
{
    public class AccountRepository
    {
        public IDbConnectionFactory ConnectionFactory { get; }
        public AccountRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        public Account Add(Account account)
        {
            // TODO: Move this validation up to a service, but don't put it in the controller.
            if (account is null)
                throw new ArgumentException("account cannot be null", nameof(account));

            if (string.IsNullOrWhiteSpace(account.Name))
                throw new ArgumentException("name cannot be null", nameof(account));
                
            if (string.IsNullOrWhiteSpace(account.PasswordHash))
                throw new ArgumentException("password hash cannot be null", nameof(account));

            using (var db = ConnectionFactory.CreateConnection())
            {
                // Encoding protects against null characters in the string.
                db.Execute("INSERT into accounts(id, name, passwordhash, role) VALUES(@Id, @Name, @PasswordHash, @Role);", new
                {
                    id = account.Id,
                    name = account.Name,
                    passwordhash = account.PasswordHash,
                    role = account.Role
                });

                var inserted = db.QueryFirst<Account>("select id, name, passwordhash, role from accounts where id = @id;", new { id = account.Id });
                return inserted;
            }
        }

        public void Remove(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute("DELETE FROM accounts WHERE id = @id", new { id = id });
            }
        }
    }
}