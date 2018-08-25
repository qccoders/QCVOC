// <copyright file="PatronFilters.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Domain.Patrons
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using QCVOC.Api.Common;

    /// <summary>
    ///     Patron request pagination and filtering options.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Omitted for Swashbuckle compatibility.")]
    public class PatronFilters : Filters
    {
        /// <summary>
        ///     The address of the Patron.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     The email address of the Patron.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     The starting time of an enrollment date range.
        /// </summary>
        public DateTime? EnrollmentDateStart { get; set; }

        /// <summary>
        ///     The ending time of an enrollment date range.
        /// </summary>
        public DateTime? EnrollmentDateEnd { get; set; }

        /// <summary>
        ///     The first name of the Patron.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     The id of the Patron.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        ///     The last name of the Patron.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     The starting time of a last updated date range.
        /// </summary>
        public DateTime? LastUpdateDateStart { get; set; }

        /// <summary>
        ///     The ending time of a last updated date range.
        /// </summary>
        public DateTime? LastUpdateDateEnd { get; set; }

        /// <summary>
        ///     The name of the user which performed the last update.
        /// </summary>
        public string LastUpdateBy { get; set; }

        /// <summary>
        ///     The member id of the Patron.
        /// </summary>
        public int? MemberId { get; set; }

        /// <summary>
        ///     The primary phone number of the Patron.
        /// </summary>
        public string PrimaryPhone { get; set; }

        /// <summary>
        ///     The secondary phone number of the Patron.
        /// </summary>
        public string SecondaryPhone { get; set; }
    }
}
