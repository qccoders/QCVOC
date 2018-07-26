using System;
using Xunit;
using QCVOC.Server.Data.Repository;
using QCVOC.Server.Data.ConnectionFactory;
using QCVOC.Server.Data.Model.Security;
using QCVOC.Server;
using FsCheck;
using FsCheck.Experimental;
using FsCheck.Xunit;
using System.Linq;

namespace Server.Tests
{
    public class RefreshTokens 
    {
        public RefreshTokens()
        {
          Arb.Register<Generators>();
        }

        [Fact(DisplayName = "Given a valid refresh token, it can be created, retrieved, updated and deleted.")]
        [Trait("Type", "Integration")]
        public void RefreshTokenLifecycle()
            => Prop.ForAll<RefreshToken, RefreshTokenRepository>(
                Generators.ArbRefreshToken(),
                Generators.ArbRefreshTokenRepository(),
                (token, repository) => Lifecycle(token, repository))
                .QuickCheckThrowOnFailure();

        private Property Lifecycle(RefreshToken token, RefreshTokenRepository repository)
        {
            return Insertable(token, repository)
            .And(Updateable(token, repository))
            .And(Gettable(token, repository))
            .And(Deleteable(token, repository));
        }

        private Property Insertable(RefreshToken token, RefreshTokenRepository tokens)
        {
            var inserted = tokens.Create(token);
            var equal = inserted.Equals(token);
            return equal.ToProperty();
        }

        private Property Updateable(RefreshToken token, RefreshTokenRepository tokens)
        {
            token.TokenId = Guid.NewGuid();
            token.Issued = DateTime.Now;
            token.Expires = DateTime.Now;
            var updated = tokens.Update(token);
            var equal = updated.Equals(token);
            return equal.ToProperty();
        }
        private Property Gettable(RefreshToken token, RefreshTokenRepository tokens)
            => (tokens.GetAll().Count() > 0).ToProperty();
        private Property Deleteable(RefreshToken token, RefreshTokenRepository tokens)
        {
            tokens.Delete(token);
            var equal = (tokens.Get(token.AccountId) == null);
            return equal.ToProperty();
        }
    }
}
