using System;
using Xunit;
using QCVOC.Server.Data;
using QCVOC.Server.Data.ConnectionFactory;
using QCVOC.Server.Data.Model.Security;
using QCVOC.Server;
using FsCheck;
using FsCheck.Experimental;
using FsCheck.Xunit;

namespace Server.Tests
{
    public class Accounts
    {
        private IDbConnectionFactory ConnectionFactory { get; }
        public Accounts()
        {
            var connectionString = Environment.GetEnvironmentVariable("qcvoc_connectionstring");
            ConnectionFactory = new NpgsqlDbConnectionFactory(connectionString);
        }

        [Property(DisplayName = "Given an account, when it's created then it can be deleted."), Trait("Type", "Integration")]
        public Property CreateAccount()
        {
            Func<Account, Boolean> badClassification = (account) 
                => string.IsNullOrWhiteSpace(account?.Name) || string.IsNullOrWhiteSpace(account?.PasswordHash);

            return Prop.ForAll<Account>(account =>
            {
                if (!badClassification(account))
                {
                    var accounts = new AccountRepository(ConnectionFactory);
                    var inserted = accounts.Add(account);
                    Assert.True(inserted.Id == account.Id);
                    accounts.Remove(inserted.Id);
                }
                else
                {
                    Assert.Throws<ArgumentException>(() =>
                    {
                        var accounts = new AccountRepository(ConnectionFactory);
                        var inserted = accounts.Add(account);
                        Assert.True(inserted.Id == account.Id);
                        accounts.Remove(inserted.Id);
                    });
                }

            });

        }
    }
}
