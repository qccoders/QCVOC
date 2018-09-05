// <copyright file="ServiceFilters.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Service
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using QCVOC.Api.Common;

    /// <summary>
    ///     Service request filtering options.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Omitted for Swashbuckle compatibility.")]
    public class ServiceFilters : Filters
    {
        /// <summary>
        ///     The Id of the Service.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     The name of the Service.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The description of the Service.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The name of the user who created the Service.
        /// </summary>
        public string CreationBy { get; set; }

        /// <summary>
        ///     The id of the user who created the Service.
        /// </summary>
        public string CreationById { get; set; }

        /// <summary>
        ///     The starting time of a creation date range.
        /// </summary>
        public string CreationDateStart { get; set; }

        /// <summary>
        ///     The ending time of a creation date range.
        /// </summary>
        public string CreationDateEnd { get; set; }

    }
}
