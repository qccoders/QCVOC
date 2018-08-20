// <copyright file="Patron.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Domain.Patrons.Data.Model
{
    using System;

    public class Patron : IEquatable<Patron>
    {
        public Patron()
        {
        }

        public Patron(
            Guid id,
            int memberId,
            string firstName,
            string lastName,
            string address,
            string primaryPhone,
            string secondaryPhone,
            string email,
            DateTime enrollmentDate)
        {
            Id = id;
            MemberId = memberId;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            PrimaryPhone = primaryPhone;
            SecondaryPhone = secondaryPhone;
            Email = email;
            EnrollmentDate = enrollmentDate;
        }

        public string Address { get; set; }

        public string Email { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public string FirstName { get; set; }

        public Guid Id { get; set; }

        public string LastName { get; set; }
        public int MemberId { get; set; }
        public string PrimaryPhone { get; set; }
        public string SecondaryPhone { get; set; }

        public bool Equals(Patron patron)
        {
            if (patron == null)
            {
                return this == null;
            }

            return this.Id == patron.Id
            && this.MemberId == patron.MemberId
            && this.FirstName == patron.FirstName
            && this.LastName == patron.LastName
            && this.Address == patron.Address
            && this.PrimaryPhone == patron.PrimaryPhone
            && this.SecondaryPhone == patron.SecondaryPhone
            && this.Email == patron.Email
            && this.EnrollmentDate - patron.EnrollmentDate <= TimeSpan.FromSeconds(1);
        }
    }
}