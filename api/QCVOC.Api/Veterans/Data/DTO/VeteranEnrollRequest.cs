// <copyright file="VeteranEnrollRequest.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Veterans.Data.DTO
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     DTO containing new Veteran details for a Veteran enrollment request.
    /// </summary>
    public class VeteranEnrollRequest
    {
        /// <summary>
        ///     Gets or sets the address of the Veteran.
        /// </summary>
        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 5)]
        public string Address { get; set; }

        /// <summary>
        ///     Gets or sets the email address of the Veteran.
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the first name of the Veteran.
        /// </summary>
        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 1)]
        public string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets the last name of the Veteran.
        /// </summary>
        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 1)]
        public string LastName { get; set; }

        /// <summary>
        ///     Gets or sets the number of the card presently assigned to the Veteran.
        /// </summary>
        [Range(minimum: 0, maximum: 9999)]
        public int? CardNumber { get; set; }

        /// <summary>
        ///     Gets or sets the primary phone number of the Veteran.
        /// </summary>
        [Required]
        [RegularExpression(@"^[1-9][0-9]{9}$")]
        public string PrimaryPhone { get; set; }

        /// <summary>
        ///     Gets or sets the method used to verify eligibility of the Veteran.
        /// </summary>
        [Required]
        public VerificationMethod VerificationMethod { get; set; }

        /// <summary>
        ///     Gets or sets the photo of the veteran.
        /// </summary>
        public string PhotoBase64 { get; set; }
    }
}
