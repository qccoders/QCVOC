// <copyright file="VeteranFilters.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
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
        ///     Gets the name of the user which enrolled the Veteran.
        /// </summary>
        public string EnrollmentBy { get; }

        /// <summary>
        ///     The id of the user which enrolled the Veteran.
        /// </summary>
        public string EnrollmentById { get; set; }

        /// <summary>
        ///     The starting time of an enrollment date range.
        /// </summary>
        public DateTime? EnrollmentDateStart { get; set; }

        /// <summary>
        ///     The ending time of an enrollment date range.
        /// </summary>
        public DateTime? EnrollmentDateEnd { get; set; }

        /// <summary>
        ///     The first name of the Veteran.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     The id of the Veteran.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        ///     The last name of the Veteran.
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
        ///     The id of the user which performed the last update.
        /// </summary>
        public Guid? LastUpdateById { get; set; }

        /// <summary>
        ///     The number of the card presently assigned to the Veteran.
        /// </summary>
        public int? CardNumber { get; set; }

        /// <summary>
        ///     The primary phone number of the Veteran.
        /// </summary>
        public string PrimaryPhone { get; set; }
    }
}
