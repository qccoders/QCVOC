// <copyright file="EventFilters.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Events.Data.Model
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using QCVOC.Api.Common;

    /// <summary>
    ///     An Event.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Omitted for Swashbuckle compatibility.")]
    public class EventFilters : Filters
    {
        /// <summary>
        ///     The id of the Event.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        ///     The name of the Event.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The ending time of the Event date range.
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        ///     The starting time of the Event date range.
        /// </summary>
        public DateTime? DateStart { get; set; }
    }
}