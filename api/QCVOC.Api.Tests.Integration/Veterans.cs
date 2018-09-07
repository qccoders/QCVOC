// <copyright file="Veterans.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace Server.Tests
{
    using System;
    using System.Linq;
    using FsCheck;
    using FsCheck.Experimental;
    using FsCheck.Xunit;
    using QCVOC.Api.Veterans.Data.Model;
    using QCVOC.Api.Veterans.Data.Repository;
    using Xunit;

    public class Veterans
    {
        public Veterans()
        {
            Arb.Register<Generators>();
        }

        [Fact(DisplayName = "Given a valid veteran, it can be created, retrieved, updated and deleted.")]
        [Trait("Type", "Integration")]
        public void VeteranLifecycle()
            => Prop.ForAll<Veteran, VeteranRepository>(
                Generators.ArbVeteran(),
                Generators.ArbVeteranRepository(),
                (veteran, repository) => Lifecycle(veteran, repository)).QuickCheckThrowOnFailure();

        private Property Lifecycle(Veteran veteran, VeteranRepository repository)
        {
            return Insertable(veteran, repository)
            .And(Updateable(veteran, repository))
            .And(Gettable(veteran, repository))
            .And(Deleteable(veteran, repository));
        }

        private Property Insertable(Veteran veteran, VeteranRepository veterans)
        {
            var inserted = veterans.Create(veteran);
            var equal = inserted.Equals(veteran);
            return equal.ToProperty();
        }

        private Property Updateable(Veteran veteran, VeteranRepository veterans)
        {
            veteran.FirstName = "TestFirstName";
            veteran.LastName = "TestLastName";
            veteran.MemberId = 1234567;
            veteran.Address = "1111 1st street";
            veteran.PrimaryPhone = "(123) 123-1234";
            veteran.Email = "test@qcvoc.com";
            veteran.EnrollmentDate = DateTime.Now;

            var updated = veterans.Update(veteran);
            var equal = updated.Equals(veteran);
            return equal.ToProperty();
        }

        private Property Gettable(Veteran veteran, VeteranRepository veterans)
            => (veterans.GetAll().Count() > 0).ToProperty();

        private Property Deleteable(Veteran veteran, VeteranRepository veterans)
        {
            veterans.Delete(veteran);
            var equal = veterans.Get(veteran.Id) == null;
            return equal.ToProperty();
        }
    }
}
