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
using QCVOC.Server.Data.Model;

namespace Server.Tests
{
    public class Patrons 
    {
        public Patrons()
        {
            Arb.Register<Generators>();
        }

        [Fact(DisplayName = "Given a valid patron, it can be created, retrieved, updated and deleted.")]
        [Trait("Type", "Integration")]
        public void PatronLifecycle()
            => Prop.ForAll<Patron, PatronRepository>(
                Generators.ArbPatron(),
                Generators.ArbPatronRepository(),
                (patron, repository) => Lifecycle(patron, repository)).QuickCheckThrowOnFailure();

        private Property Lifecycle(Patron patron, PatronRepository repository)
        {
            return Insertable(patron, repository)
            .And(Updateable(patron, repository))
            .And(Gettable(patron, repository))
            .And(Deleteable(patron, repository));
        }

        private Property Insertable(Patron patron, PatronRepository patrons)
        {
            var inserted = patrons.Create(patron);
            var equal = inserted.Equals(patron);
            return equal.ToProperty();
        }

        private Property Updateable(Patron patron, PatronRepository patrons)
        {
            patron.FirstName = "TestFirstName";
            patron.LastName = "TestLastName";
            patron.MemberId = 1234567;
            patron.Address = "1111 1st street";
            patron.PrimaryPhone = "(123) 123-1234";
            patron.SecondaryPhone= "(321) 321-4321";
            patron.Email = "test@qcvoc.com";
            patron.EnrollmentDate = DateTime.Now;

            var updated = patrons.Update(patron);
            var equal = updated.Equals(patron);
            return equal.ToProperty();
        }
        private Property Gettable(Patron patron, PatronRepository patrons)
            => (patrons.GetAll().Count() > 0).ToProperty();
        private Property Deleteable(Patron patron, PatronRepository patrons)
        {
            patrons.Delete(patron);
            var equal = (patrons.Get(patron.Id) == null);
            return equal.ToProperty();
        }
    }
}
