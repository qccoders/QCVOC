using System;
using Xunit;
using QCVOC.Api.Data.Repository;
using QCVOC.Api.Data.ConnectionFactory;
using QCVOC.Api.Data.Model.Security;
using QCVOC.Api;
using FsCheck;
using FsCheck.Experimental;
using FsCheck.Xunit;
using System.Linq;

namespace Server.Tests
{
    public class Accounts
    {
        public Accounts()
        {
            Arb.Register<Generators>();
        }

        [Fact(DisplayName = "Given a valid account, it can be created, retrieved, updated and deleted.")]
        [Trait("Type", "Integration")]
        public void AccountLifecycle()
            => Prop.ForAll<Account, AccountRepository>(
                Generators.ArbAccount(),
                Generators.ArbAccountRepository(),
                (account, repository) => Lifecycle(account, repository)).QuickCheckThrowOnFailure();

        private Property Lifecycle(Account account, AccountRepository repository)
        {
            return Insertable(account, repository)
            .And(Updateable(account, repository))
            .And(Gettable(account, repository))
            .And(Deleteable(account, repository));
        }

        private Property Insertable(Account account, AccountRepository accounts)
        {
            var inserted = accounts.Create(account);
            return inserted.Equals(account).ToProperty();
        }

        private Property Updateable(Account account, AccountRepository accounts)
        {
            account.Name = "Test";
            account.PasswordHash = "Test";
            account.Role = Role.User;
            var updated = accounts.Update(account);
            return updated.Equals(account).ToProperty();
        }
        private Property Gettable(Account account, AccountRepository accounts)
            => (accounts.GetAll().Count() > 0).ToProperty();
        private Property Deleteable(Account account, AccountRepository accounts)
        {
            accounts.Delete(account);
            return (accounts.Get(account.Id) == null).ToProperty();
        }
    }
}
