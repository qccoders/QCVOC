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
    public class AccountRepository : IRepository<Account>
    {
        public IDbConnectionFactory ConnectionFactory { get; }
        public AccountRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        public Account Create(Account account)
        {
            // TODO: Move this validation up to a service, but don't put it in the controller.
            if (account == null)
                throw new ArgumentException("account cannot be null", nameof(account));

            if (string.IsNullOrWhiteSpace(account.Name))
                throw new ArgumentException("name cannot be null", nameof(account));

            if (string.IsNullOrWhiteSpace(account.PasswordHash))
                throw new ArgumentException("password hash cannot be null", nameof(account));

            if (account.Name.Contains("\0"))
                throw new ArgumentException("null characters are not allowd in an account name.", nameof(account));

            if (account.PasswordHash.Contains("\0"))
                throw new ArgumentException("null characters are not allowed in an password hash.", nameof(account));

            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute("INSERT into accounts(id, name, passwordhash, role) VALUES(@Id, @Name, @PasswordHash, @Role);", new
                {
                    id = account.Id,
                    name = account.Name,
                    passwordhash = account.PasswordHash,
                    role = account.Role
                });

                var inserted = Get(account.Id);
                return inserted;
            }
        }

        public void Delete(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute("DELETE FROM accounts WHERE id = @id", new { id = id });
            }
        }

        public void Delete(Account account)
        { 
            if(account == null)
                throw new ArgumentException("account cannot be null.", nameof(account));

            Delete(account.Id);
        }

        public Account Get(Guid id)
        {
            using(var db = ConnectionFactory.CreateConnection())
            {
                return db.QueryFirstOrDefault<Account>("SELECT id, name, passwordhash, role FROM accounts where id = @id;", new { id = id });
            }
        }

        public IEnumerable<Account> GetAll()
        {
            //TODO: Add paging when necessary.
            using(var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<Account>("SELECT id, name, passwordhash, role FROM accounts;");
            }
        }

        public Account Update(Account account)
        {
            using(var db = ConnectionFactory.CreateConnection())
            {
                db.Execute("UPDATE accounts set name = @name, passwordhash = @passwordhash, role = @role WHERE id = @id;", new {
                    name = account.Name,
                    passwordhash = account.PasswordHash,
                    role = account.Role,
                    id = account.Id
                });

                return Get(account.Id);
            }
        }
    }
}