// <copyright file="PatronCreateRequest.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Domain.Patrons.Data.DTO
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     DTO containing new Patron details for a Patron enrollment request.
    /// </summary>
    public class PatronEnrollRequest
    {
        /// <summary>
        ///     Gets or sets the address of the Patron.
        /// </summary>
        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 5)]
        public string Address { get; set; }

        /// <summary>
        ///     Gets or sets the email address of the Patron.
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the first name of the Patron.
        /// </summary>
        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 1)]
        public string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets the last name of the Patron.
        /// </summary>
        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 1)]
        public string LastName { get; set; }

        /// <summary>
        ///     Gets or sets the member id of the Patron.
        /// </summary>
        [Required]
        [Range(minimum: 1000, maximum: 9999)]
        public int MemberId { get; set; }

        /// <summary>
        ///     Gets or sets the primary phone number of the Patron.
        /// </summary>
        [Required]
        [Phone]
        public string PrimaryPhone { get; set; }

        /// <summary>
        ///     Gets or sets the secondary phone number of the Patron.
        /// </summary>
        [Phone]
        public string SecondaryPhone { get; set; }
    }
}