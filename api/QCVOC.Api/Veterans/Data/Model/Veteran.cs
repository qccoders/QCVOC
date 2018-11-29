// <copyright file="Veteran.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Veterans.Data.Model
{
    using System;

    /// <summary>
    ///     A Veteran.
    /// </summary>
    public class Veteran : IEquatable<Veteran>
    {
        /// <summary>
        ///     Gets or sets the address of the Veteran.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     Gets or sets the email address of the Veteran.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Gets the name of the user which enrolled the Veteran.
        /// </summary>
        public string EnrollmentBy { get; }

        /// <summary>
        ///     Gets or sets the id of the user which enrolled the Veteran.
        /// </summary>
        public Guid EnrollmentById { get; set; }

        /// <summary>
        ///     Gets or sets the date on which the Veteran was enrolled.
        /// </summary>
        public DateTime EnrollmentDate { get; set; }

        /// <summary>
        ///     Gets or sets the photo of the veteran.
        /// </summary>
        public string PhotoBase64 { get; set; }

        /// <summary>
        ///     Gets or sets the first name of the Veteran.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Gets the full name of the Veteran.
        /// </summary>
        public string FullName => FirstName + " " + LastName;

        /// <summary>
        ///     Gets or sets the id of the Veteran.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the last name of the Veteran.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Gets or sets the date on which the Veteran was last updated.
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        ///     Gets the name of the user which performed the last update.
        /// </summary>
        public string LastUpdateBy { get; }

        /// <summary>
        ///     Gets or sets the id of the user which performed the last update.
        /// </summary>
        public Guid LastUpdateById { get; set; }

        /// <summary>
        ///     Gets or sets the number of the card presently assigned to the Veteran.
        /// </summary>
        public int? CardNumber { get; set; }

        /// <summary>
        ///     Gets or sets the primary phone number of the Veteran.
        /// </summary>
        public string PrimaryPhone { get; set; }

        /// <summary>
        ///     Gets or sets the method used to verify eligibility of the Veteran.
        /// </summary>
        public VerificationMethod VerificationMethod { get; set; }

        /// <summary>
        ///     Compares two Veteran instances.
        /// </summary>
        /// <param name="veteran">The Veteran to which to compare.</param>
        /// <returns>A value indicating whether the compared instances are equal.</returns>
        public bool Equals(Veteran veteran)
        {
            if (veteran == null)
            {
                return this == null;
            }

            return this.Id == veteran.Id
            && this.CardNumber == veteran.CardNumber
            && this.FirstName == veteran.FirstName
            && this.LastName == veteran.LastName
            && this.LastUpdateDate == veteran.LastUpdateDate
            && this.LastUpdateById == veteran.LastUpdateById
            && this.LastUpdateBy == veteran.LastUpdateBy
            && this.Address == veteran.Address
            && this.PhotoBase64 == veteran.PhotoBase64
            && this.PrimaryPhone == veteran.PrimaryPhone
            && this.Email == veteran.Email
            && this.EnrollmentById == veteran.EnrollmentById
            && this.EnrollmentBy == veteran.EnrollmentBy
            && this.VerificationMethod == veteran.VerificationMethod
            && this.EnrollmentDate - veteran.EnrollmentDate <= TimeSpan.FromSeconds(1);
        }
    }
}