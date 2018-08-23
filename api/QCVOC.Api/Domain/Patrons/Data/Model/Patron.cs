// <copyright file="Patron.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Domain.Patrons.Data.Model
{
    using System;

    /// <summary>
    ///     A service Patron.
    /// </summary>
    public class Patron : IEquatable<Patron>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Patron"/> class.
        /// </summary>
        public Patron()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Patron"/> class.
        /// </summary>
        /// <param name="id">The id of the Patron</param>
        /// <param name="memberId">The member id of the Patron.</param>
        /// <param name="firstName">The first name of the Patron/</param>
        /// <param name="lastName">The last name of the Patron.</param>
        /// <param name="address">The address of the Patron.</param>
        /// <param name="primaryPhone">The primary phone number of the Patron.</param>
        /// <param name="secondaryPhone">The secondary phone number of the Patron.</param>
        /// <param name="email">The email address of the Patron.</param>
        /// <param name="enrollmentDate">The date on which the Patron was enrolled.</param>
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

        /// <summary>
        ///     Gets or sets the address of the Patron.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     Gets or sets the email address of the Patron.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the date on which the Patron was enrolled.
        /// </summary>
        public DateTime EnrollmentDate { get; set; }

        /// <summary>
        ///     Gets or sets the first name of the Patron.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets the id of the Patron.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the last name of the Patron.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Gets or sets the member id of the Patron.
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        ///     Gets or sets the primary phone number of the Patron.
        /// </summary>
        public string PrimaryPhone { get; set; }

        /// <summary>
        ///     Gets or sets the secondary phone number of the Patron.
        /// </summary>
        public string SecondaryPhone { get; set; }

        /// <summary>
        ///     Compares two Patron instances.
        /// </summary>
        /// <param name="patron">The Patron to which to compare.</param>
        /// <returns>A value indicating whether the compared instances are equal.</returns>
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