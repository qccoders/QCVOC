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