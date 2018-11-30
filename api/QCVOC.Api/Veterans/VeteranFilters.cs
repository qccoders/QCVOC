// <copyright file="VeteranFilters.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Veterans
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using QCVOC.Api.Common;

    /// <summary>
    ///     Veteran request pagination and filtering options.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Omitted for Swashbuckle compatibility.")]
    public class VeteranFilters : Filters
    {
        /// <summary>
        ///     The address of the Veteran.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     The email address of the Veteran.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     The first name of the Veteran.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     The id of the Veteran.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        ///     A value indicating whether the PhotoBase64 data should be included in the results.
        /// </summary>
        public bool IncludePhotoBase64 { get; set; }

        /// <summary>
        ///     The last name of the Veteran.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     The number of the card presently assigned to the Veteran.
        /// </summary>
        public int? CardNumber { get; set; }

        /// <summary>
        ///     The method used to verify eligibility of the Veteran.
        /// </summary>
        public VerificationMethod? VerificationMethod { get; set; }

        /// <summary>
        ///     The primary phone number of the Veteran.
        /// </summary>
        public string PrimaryPhone { get; set; }
    }
}
