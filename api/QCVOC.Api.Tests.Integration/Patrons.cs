// <copyright file="Patrons.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace Server.Tests
{
    using System;
    using System.Linq;
    using FsCheck;
    using FsCheck.Experimental;
    using FsCheck.Xunit;
    using QCVOC.Api.Patrons.Data.Model;
    using QCVOC.Api.Patrons.Data.Repository;
    using Xunit;

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
            patron.SecondaryPhone = "(321) 321-4321";
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
            var equal = patrons.Get(patron.Id) == null;
            return equal.ToProperty();
        }
    }
}
