using System;
using Xunit;
using QCVOC.Server.Data;
using QCVOC.Server.Data.ConnectionFactory;
using QCVOC.Server.Data.Model.Security;
using QCVOC.Server;
using FsCheck;
using FsCheck.Experimental;
using QCVOC.Server.Data.Repository;

namespace Server.Tests
{
    public class Generators
    {
        public static Arbitrary<Role> ArbRole() => Arb.From(GenRole());
        public static Arbitrary<Account> ArbAccount() => Arb.From(GenAccount());
        public static Arbitrary<AccountRepository> ArbAccountRepository() => Arb.From(GenAccountRepository());

        public static Gen<AccountRepository> GenAccountRepository()
            => from _ in Arb.Default.String().Generator 
            select new AccountRepository(new NpgsqlDbConnectionFactory(Environment.GetEnvironmentVariable("qcvoc_connectionstring")));

        public static Gen<Role> GenRole()
            => Gen.Elements(new[] { Role.Administrator, Role.Supervisor, Role.User });

        public static Gen<string> GenSHA512Hash()
            => from a in Arb.Default.NonEmptyString().Generator.Where(p => !p.ToString().Contains("\0")) 
            select Utility.ComputeSHA512Hash(a.ToString());

        public static Gen<Account> GenAccount()
        {
            return from id in Arb.Default.Guid().Generator
                   from username in Arb.Default.NonEmptyString().Generator
                   from password in GenSHA512Hash()
                   from role in GenRole()
                   select new Account()
                   {
                       Id = id,
                       Name = username.ToString(),
                       PasswordHash = password,
                       Role = role
                   };
        }
    }
}