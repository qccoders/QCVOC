// <copyright file="ScanFilters.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Scans
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using QCVOC.Api.Common;

    /// <summary>
    ///     Scan request pagination and filtering options.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Omitted for Swashbuckle compatibility.")]
    public class ScanFilters : Filters
    {
        /// <summary>
        ///     The id of the Event.
        /// </summary>
        public Guid? EventId { get; set; }

        /// <summary>
        ///     The id of the Veteran.
        /// </summary>
        public Guid? VeteranId { get; set; }

        /// <summary>
        ///     The id of the Service.
        /// </summary>
        public Guid? ServiceId { get; set; }

        /// <summary>
        ///     The starting time of a Scan date range.
        /// </summary>
        public DateTime? ScanDateStart { get; set; }

        /// <summary>
        ///     The ending time of a Scan date range.
        /// </summary>
        public DateTime? ScanDateEnd { get; set; }

        /// <summary>
        ///     A value indicating whether the Veteran brought a guest.
        /// </summary>
        public bool? PlusOne { get; set; }
    }
}
