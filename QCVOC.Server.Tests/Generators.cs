using System;
using Xunit;
using QCVOC.Server.Data;
using QCVOC.Server.Data.ConnectionFactory;
using QCVOC.Server.Data.Model.Security;
using QCVOC.Server;
using FsCheck;
using FsCheck.Experimental;

namespace Server.Tests
{
    public class Generators
    {
        public static Gen<Role> GenRole()
            => Gen.Elements(new[] { Role.Administrator, Role.Supervisor, Role.User });

        public static Gen<string> GenSHA512Hash()
            => from a in Arb.Default.String().Generator select Utility.ComputeSHA512Hash(a);
            
        public static Gen<Account> GenAccount()
        {
            return from id in Arb.Default.Guid().Generator
                   from username in Arb.Default.String().Generator
                   from password in Arb.Default.String().Generator
                   from role in GenRole()
                   select new Account() { Id = id, Name = username, PasswordHash = password, Role = role };
        }
    }
}